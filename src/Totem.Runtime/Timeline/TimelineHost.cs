using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The hosting of timeline activity by a runtime
	/// </summary>
	public sealed class TimelineHost : TimelineScope, ITimelineHost
	{
		private readonly ITimelineDb _db;
		private readonly Func<FlowType, IFlowHost> _flowFactory;
		private ConcurrentDictionary<FlowType, IFlowHost> _flowsByType;
		private ConcurrentDictionary<Id, TimelineRequest> _requestsById;

		public TimelineHost(ITimelineDb db, Func<FlowType, IFlowHost> flowFactory)
		{
			_db = db;
			_flowFactory = flowFactory;
		}

		//
		// Lifecycle
		//

		protected override void Open()
		{
			base.Open();

			_flowsByType = new ConcurrentDictionary<FlowType, IFlowHost>();
			_requestsById = new ConcurrentDictionary<Id, TimelineRequest>();

			ResumeFlows();
		}

		protected override void Close()
		{
			base.Close();

			_flowsByType = null;
			_requestsById = null;
		}

		private void ResumeFlows()
		{
			var resumeInfo = _db.ReadResumeInfo();

			foreach(var flow in resumeInfo.Flows)
			{
				HostFlow(flow);
			}

			foreach(var point in resumeInfo.Points)
			{
				Push(point);
			}
		}

		private void HostFlow(FlowType type)
		{
			var flow = _flowFactory(type);

			_flowsByType[type] = flow;

			RemoveFlowWhenDone(flow);

			flow.Connect(State.CancellationToken);
		}

		private void RemoveFlowWhenDone(IFlowHost flow)
		{
			flow.Task.ContinueWith(_ =>
			{
				IFlowHost removed;

				_flowsByType.TryRemove(flow.Type, out removed);
			});
		}

		//
		// CallWhen
		//

		protected override Task CallWhen()
		{
			if(Point.Scheduled)
			{
				ScheduleAppend();
			}
			//else
			//{
				HostNewFlows();

				PushToFlows();

				PushToRequest();
			//}

			return Task.FromResult(0);
		}

		private void ScheduleAppend()
		{
			var scheduledPoint = Point;
			var when = new DateTimeOffset(scheduledPoint.Event.When);

			// Embed subscription lifetime into the callback

			IDisposable subscription = null;

			subscription = Observable.Timer(when).Subscribe(_ =>
			{
				subscription.Dispose();

				Push(_db.AppendOccurred(scheduledPoint));
			});
		}

		//
		// Push
		//

		private void HostNewFlows()
		{
			var currentFlows = _flowsByType.Keys.ToHashSet();

			var newFlows =
				from region in Runtime.Regions
				from package in region.Packages
				from flow in package.Flows
				where !flow.IsRequest
				where flow.CanCall(Point.EventType)
				where !currentFlows.Contains(flow)
				select flow;

			foreach(var newFlow in newFlows)
			{
				HostFlow(newFlow);
			}
		}

		private void PushToFlows()
		{
			foreach(var flow in _flowsByType.Values)
			{
				flow.Push(Point);
			}
		}

		private void PushToRequest()
		{
			var requestId = Flow.Traits.RequestId.Get(Point.Event);

			if(requestId.IsAssigned)
			{
				TimelineRequest request;

				if(_requestsById.TryGetValue(requestId, out request))
				{
					request.Push(Point);
				}
			}
		}

		//
		// Requests
		//

		public async Task<TFlow> MakeRequest<TFlow>(Id id) where TFlow : RequestFlow
		{
			EnsureUniqueRequestId(id);

			var request = AddRequest<TFlow>(id);

			try
			{
				return await request.Task;
			}
			finally
			{
				RemoveRequest(id);
			}
		}

		private void EnsureUniqueRequestId(Id id)
		{
			if(_requestsById.ContainsKey(id))
			{
				throw new InvalidOperationException(Text.Of("Request id {0} is already in progress", id));
			}
		}

		private TimelineRequest<TFlow> AddRequest<TFlow>(Id id) where TFlow : RequestFlow
		{
			var request = CreateRequest<TFlow>();

			_requestsById[id] = request;

			return request;
		}

		private TimelineRequest<TFlow> CreateRequest<TFlow>() where TFlow : RequestFlow
		{
			var type = Runtime.GetFlow(typeof(TFlow));

			var flow = _flowFactory(type);

			flow.Connect(State.CancellationToken);

			return new TimelineRequest<TFlow>(flow);
		}

		private void RemoveRequest(Id id)
		{
			TimelineRequest request;

			_requestsById.TryRemove(id, out request);
		}
	}
}