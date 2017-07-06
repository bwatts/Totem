using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of timeline activity in a runtime
  /// </summary>
  public sealed class TimelineScope : Connection, ITimelineScope
	{
    readonly ILifetimeScope _lifetime;
    readonly ITimelineDb _timelineDb;
		readonly IViewExchange _viewExchange;
		readonly TimelineSchedule _schedule;
		readonly TimelineFlowSet _flows;
		readonly TimelineRequestSet _requests;
		readonly TimelineQueue _queue;

    public TimelineScope(ILifetimeScope lifetime, ITimelineDb timelineDb, IViewExchange viewExchange)
		{
      _lifetime = lifetime;
      _timelineDb = timelineDb;
			_viewExchange = viewExchange;

      _schedule = new TimelineSchedule(this);
      _flows = new TimelineFlowSet(this);
      _requests = new TimelineRequestSet(this);

			_queue = new TimelineQueue(_timelineDb, _schedule, _flows, _requests);
    }

    protected override void Open()
		{
      Track(_schedule);
      Track(_flows);
      Track(_requests);
			Track(_queue);
		}

    public async Task Execute(Request request, User user)
		{
      Expect(request.Id.IsAssigned, "Request must be assigned an identifier");

      var scope = new RequestScope(_lifetime, this, request, user);

      var startEvent = await scope.CallStart();

      var executeTask = _requests.Execute(scope);

      await Push(TimelinePosition.None, startEvent);

      await executeTask;
    }

		public async Task Push(TimelinePosition cause, Event e)
		{
      using(var enqueue = _queue.StartEnqueue())
      using(var _ = TimelineMetrics.EnqueueTime.Measure())
      {
        enqueue.Commit(await _timelineDb.Push(cause, e));
      }
    }

    internal async Task PushScheduled(TimelinePoint point)
		{
      using(var enqueue = _queue.StartEnqueue())
      using(var _ = TimelineMetrics.EnqueueTime.Measure(point.ToPath("scheduled")))
      {
        enqueue.Commit(await _timelineDb.PushScheduled(point));
      }
    }

    internal async Task PushStopped(FlowPoint point, Exception error)
    {
      using(var enqueue = _queue.StartEnqueue())
      using(var _ = TimelineMetrics.EnqueueTime.Measure(point.ToPath("stopped")))
      {
        enqueue.Commit(await _timelineDb.PushStopped(point, error));
      }
    }

    internal async Task<PushTopicResult> PushTopic(Topic topic, FlowPoint point, IEnumerable<Event> newEvents)
		{
      var metricsPath = topic.ToPath(point.Position);

      using(var enqueue = _queue.StartEnqueue())
      using(var _ = TimelineMetrics.PushTime.Measure(metricsPath))
      {
        var result = await _timelineDb.PushTopic(topic, point, newEvents);

        using(var __ = TimelineMetrics.EnqueueTime.Measure(metricsPath))
        {
          enqueue.Commit(result.Messages);
        }

        return result;
      }
    }

    internal Task PushView(View view)
    {
      return _timelineDb.PushView(view);
    }

    internal void TryPushRequestError(TimelinePoint point, Exception error)
    {
      _requests.TryPushError(point, error);
    }

    internal IFlowScope OpenFlowScope(FlowRoute route)
    {
      if(route.Key.Type.IsTopic)
      {
        return new TopicScope(_lifetime, this, route);
      }
      else if(route.Key.Type.IsView)
      {
        return new ViewScope(_lifetime, this, _viewExchange, route);
      }
      else
      {
        return new FlowScope(_lifetime, this, route);
      }
    }

    internal Task<Flow> ReadFlow(FlowRoute route, bool strict = true)
    {
      return _timelineDb.ReadFlow(route, strict);
    }
	}
}