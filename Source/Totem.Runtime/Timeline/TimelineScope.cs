using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The scope of timeline activity in a runtime
	/// </summary>
	public sealed class TimelineScope : Connection, ITimelineScope
	{
		private readonly ILifetimeScope _lifetime;
		private readonly ITimelineDb _timelineDb;
		private readonly IFlowDb _flowDb;
		private readonly IViewExchange _viewExchange;
		private readonly TimelineSchedule _schedule;
		private readonly TimelineFlowSet _flows;
		private readonly TimelineRequestSet _requests;
		private readonly TimelineQueue _queue;

		public TimelineScope(
			ILifetimeScope lifetime,
			ITimelineDb timelineDb,
			IFlowDb flowDb,
			IViewExchange viewExchange)
		{
			_lifetime = lifetime;
			_timelineDb = timelineDb;
			_flowDb = flowDb;
			_viewExchange = viewExchange;

			_schedule = new TimelineSchedule(this);
			_flows = new TimelineFlowSet(this);
			_requests = new TimelineRequestSet(this);

			_queue = new TimelineQueue(_schedule, _flows, _requests);
		}

		protected override void Open()
		{
			Track(_schedule);
			Track(_flows);
			Track(_requests);
			Track(_queue);

			var resumeInfo = _timelineDb.ReadResumeInfo();

			_schedule.ResumeWith(resumeInfo);
			_queue.ResumeWith(resumeInfo);
		}

		//
		// Runtime
		//

		public void Push(TimelinePoint point)
		{
			_queue.Enqueue(point);
		}

		internal void PushScheduled(TimelinePoint point)
		{
			Push(_timelineDb.WriteScheduled(point));
		}

		public Task<T> MakeRequest<T>(Id id) where T : Request
		{
			return _requests.MakeRequest<T>(id);
		}

		public bool TryOpenFlowScope(FlowType type, TimelineRoute route, out IFlowScope scope)
		{
			var unroutedScope = new FlowScope(_lifetime, _flowDb, _viewExchange, type, route);

			scope = unroutedScope.TryRoute() ? unroutedScope : null;

			return scope != null;
		}
	}
}