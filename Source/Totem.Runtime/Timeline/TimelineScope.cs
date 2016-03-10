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
    private TimelineSchedule _schedule;
    private TimelineFlowSet _flows;
    private TimelineRequestSet _requests;

    public TimelineScope(ILifetimeScope lifetime, ITimelineDb timelineDb, IFlowDb flowDb)
		{
      _lifetime = lifetime;
      _timelineDb = timelineDb;
      _flowDb = flowDb;

      _schedule = new TimelineSchedule(this);
      _flows = new TimelineFlowSet(this);
      _requests = new TimelineRequestSet(this);
    }

    protected override void Open()
		{
      Track(_schedule);
      Track(_flows);
      Track(_requests);

      ResumeTimeline();
    }

    private void ResumeTimeline()
    {
      var resumeInfo = _timelineDb.ReadResumeInfo();

      _schedule.ResumeWith(resumeInfo);

			foreach(var resumePoint in resumeInfo.Points)
			{
				Push(resumePoint.Point);
			}
    }

    //
    // Runtime
    //

    public void Push(TimelinePoint point)
    {
      _schedule.Push(point);
      _flows.Push(point);
      _requests.Push(point);
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
			var unroutedScope = new FlowScope(_lifetime, _flowDb, type, route);

			scope = unroutedScope.TryRoute() ? unroutedScope : null;

			return scope != null;
    }
	}
}