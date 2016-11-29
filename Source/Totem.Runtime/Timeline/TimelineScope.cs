using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

		public Task<T> MakeRequest<T>(TimelinePosition cause, Event e) where T : Request
		{
			var id = Flow.Traits.EnsureRequestId(e);

			var task = _requests.MakeRequest<T>(id);

			Push(cause, e);

			return task;
		}

		public void Push(TimelinePosition cause, Event e)
		{
      using(var enqueue = _queue.StartEnqueue())
      {
        enqueue.Commit(_timelineDb.Push(cause, e));
      }
    }

    internal void PushFromSchedule(TimelineMessage message)
		{
      using(var enqueue = _queue.StartEnqueue())
      {
        enqueue.Commit(_timelineDb.PushScheduled(message));
      }
    }

    internal void PushStopped(FlowPoint point, Exception error)
    {
      using(var enqueue = _queue.StartEnqueue())
      {
        enqueue.Commit(_timelineDb.PushStopped(point, error));
      }
    }

    internal PushTopicResult PushTopic(Topic topic, FlowPoint point, IEnumerable<Event> newEvents)
		{
      using(var enqueue = _queue.StartEnqueue())
      {
        var result = _timelineDb.PushTopic(topic, point, newEvents);

        enqueue.Commit(result.Messages);

        return result;
      }
    }

    internal void PushView(View view)
    {
      _timelineDb.PushView(view);
    }

    internal void TryPushRequestError(TimelinePoint point, Exception error)
    {
      _requests.TryPushError(point, error);
    }

    internal ClaimsPrincipal ReadPrincipal(FlowPoint point)
		{
			return new ClaimsPrincipal();
		}

    internal IFlowScope CreateFlow(FlowRoute route)
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

		internal RequestScope CreateRequest<T>(Id id) where T : Request
		{
			var type = Runtime.GetRequest(typeof(T));

      var key = FlowKey.From(type, id);

      var initialRoute = new FlowRoute(key, first: true, when: true, given: false, then: false);

      return new RequestScope(_lifetime, this, initialRoute);
		}

    internal bool TryReadFlow(FlowRoute route, out Flow flow)
    {
      if(route.Key.Type.IsRequest)
      {
        flow = route.Key.Type.New();

        FlowContext.Bind(flow, route.Key);

        return true;
      }

      return _timelineDb.TryReadFlow(route, out flow);
    }
	}
}