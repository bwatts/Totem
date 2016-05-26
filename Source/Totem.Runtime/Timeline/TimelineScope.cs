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
		private TimelineQueue _queue;
    private TimelineSchedule _schedule;
    private TimelineFlowSet _flows;
    private TimelineRequestSet _requests;

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
    }

    protected override void Open()
		{
      Track(_schedule);
      Track(_flows);
      Track(_requests);

			var resumeInfo = _timelineDb.ReadResumeInfo();

			_schedule.ResumeWith(resumeInfo);

			_queue = new TimelineQueue(_schedule, _flows, _requests);

			_queue.ResumeWith(resumeInfo);

			Track(_queue);
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