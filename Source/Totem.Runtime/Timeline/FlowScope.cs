using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The scope of a flow's activity on the timeline
	/// </summary>
	public sealed class FlowScope : Connection, IFlowScope
	{
		private readonly TaskCompletionSource<Flow> _taskCompletionSource = new TaskCompletionSource<Flow>();
		private readonly ILifetimeScope _lifetime;
		private readonly IFlowDb _db;
		private readonly FlowType _type;
		private TimelineRoute _route;
		private Flow _flow;
		private FlowQueue _queue;
		private Task _pushQueueTask;

		public FlowScope(ILifetimeScope lifetime, IFlowDb db, FlowType type, TimelineRoute route)
		{
			_lifetime = lifetime;
			_db = db;
			_type = type;
			_route = route;
			Key = type.CreateKey(route.Id);
		}

		public FlowKey Key { get; }
		public Task<Flow> Task => _taskCompletionSource.Task;

		internal bool TryRoute()
		{
			var route = _route;

			_route = null;

			return _db.TryReadFlow(Key.Type, route, out _flow);
		}

		protected override void Open()
		{
			Expect(_flow).IsNotNull("Routing failed - cannot connect scope");

			_queue = new FlowQueue();
			_pushQueueTask = PushQueue();
		}

		protected override void Close()
		{
			base.Close();

			_flow = null;
			_queue = null;
			_pushQueueTask = null;
		}

		public void Push(TimelinePoint point)
		{
			if(PushingQueue)
			{
				_queue.Enqueue(point);
			}
			else
			{
				Log.Warning(
					"[timeline] Cannot push to scope in phase {Phase} - ignoring {Point:l}",
					State.Phase,
					point);
			}
		}

		private bool PushingQueue => State.IsConnecting || State.IsConnected;

		private async Task PushQueue()
		{
			while(PushingQueue)
			{
				var point = await _queue.Dequeue();

				if(PushingQueue && point.Position > _flow.Checkpoint)
				{
					await PushFromQueue(point);
				}
			}
		}

		private async Task PushFromQueue(TimelinePoint point)
		{
			try
			{
				await MakeCall(point);
			}
			catch(Exception error)
			{
				if(Key.Type.IsRequest)
				{
					throw;
				}

				WriteError(point, error);
			}
		}

		private async Task MakeCall(TimelinePoint point)
		{
			Log.Verbose("[timeline] {Position:l} => {Flow:l}", point.Position, Key);

			using(var callScope = _lifetime.BeginCallScope())
			{
				var call = CreateCall(point, callScope);

				await _flow.MakeCall(call);

				_db.WriteCall(call);

				if(_flow.Done)
				{
					CompleteTask();
				}
			}
		}

		private WhenCall CreateCall(TimelinePoint point, ILifetimeScope scope)
		{
			var dependencies = scope.Resolve<IDependencySource>();
			var principal = _db.ReadPrincipal(point);

			if(_flow.Type.IsTopic)
			{
				return new TopicWhenCall(
					(Topic) _flow,
					point,
					dependencies,
					principal,
					State.CancellationToken);
			}

			return new WhenCall(
				_flow,
				point,
				dependencies,
				principal,
				State.CancellationToken);
		}

		private void WriteError(TimelinePoint point, Exception error)
		{
			Log.Error(error, "[timeline] Flow {Flow:l} stopped", _flow);

			try
			{
				_db.WriteError(Key, point, error);
			}
			finally
			{
				CompleteTask();
			}
		}

		private void CompleteTask()
		{
			_taskCompletionSource.TrySetResult(_flow);

			Disconnect();
		}
	}
}