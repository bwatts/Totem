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

    internal PushWhenResult PushWhen(Flow flow, FlowCall.When call)
		{
      using(var enqueue = _queue.StartEnqueue())
      {
        var result = _timelineDb.PushWhen(flow, call);

        enqueue.Commit(result.Messages);

        return result;
      }
    }

    internal void TryPushRequestError(Id requestId, Exception error)
    {
      _requests.TryPushError(requestId, error);
    }

    internal ClaimsPrincipal ReadPrincipal(FlowPoint point)
		{
			return new ClaimsPrincipal();
		}

		internal bool TryReadFlow(FlowRoute route, out IFlowScope flow)
		{
			Flow instance;

      if(!_timelineDb.TryReadFlow(route, out instance))
      {
        flow = null;
      }
      else if(route.Key.Type.IsTopic)
      {
        flow = new TopicScope(_lifetime, this, (Topic) instance);
      }
      else if(route.Key.Type.IsView)
      {
        flow = new ViewScope(_lifetime, this, (View) instance, _viewExchange);
      }
      else
      {
        throw new NotSupportedException($@"Flow type ""{route.Key.Type}"" has no associated scope type");
      }

			return flow != null;
		}

		internal RequestScope<T> CreateRequest<T>(Id id) where T : Request
		{
			var type = Runtime.GetRequest(typeof(T));

			var request = (T) type.New();

      FlowContext.Bind(request, FlowKey.From(type, id));

			return new RequestScope<T>(_lifetime, this, request);
		}
	}
}