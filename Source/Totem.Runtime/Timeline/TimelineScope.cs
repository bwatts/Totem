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
    private readonly ILifetimeScope _lifetime;
    private readonly ITimelineDb _timelineDb;
		private readonly IViewExchange _viewExchange;
		private readonly TimelineSchedule _schedule;
		private readonly TimelineFlowSet _flows;
		private readonly TimelineRequestSet _requests;
		private readonly TimelineQueue _queue;

    public TimelineScope(ILifetimeScope lifetime, ITimelineDb timelineDb, IViewExchange viewExchange)
		{
      _lifetime = lifetime;
      _timelineDb = timelineDb;
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

		public Task<T> MakeRequest<T>(Event e) where T : Request
		{
			var id = Flow.Traits.EnsureRequestId(e);

			var task = _requests.MakeRequest<T>(id);

			Push(Many.Of(e));

			return task;
		}

		public void Push(Many<Event> events)
		{
			_queue.Enqueue(_timelineDb.Push(events));
		}

		internal void PushFromSchedule(TimelineMessage message)
		{
			_queue.Enqueue(_timelineDb.PushFromSchedule(message));
		}

		internal void PushCall(WhenCall call)
		{
			_queue.Enqueue(_timelineDb.PushCall(call));
		}

		internal ClaimsPrincipal ReadPrincipal(TimelinePoint point)
		{
			return new ClaimsPrincipal();
		}

		internal bool TryReadFlow(TimelineRoute route, out IFlowScope flow)
		{
			Flow instance;

			flow = !_timelineDb.TryReadFlow(route, out instance)
				? null
				: new FlowScope(_lifetime, this, _viewExchange, instance);

			return flow != null;
		}

		internal TimelineRequest<T> CreateRequest<T>(Id id) where T : Request
		{
			var type = Runtime.GetRequest(typeof(T));

			var request = type.New();

			Flow.Initialize(request, FlowKey.From(type, id));

			return new TimelineRequest<T>(new FlowScope(_lifetime, this, _viewExchange, request));
		}

		internal void PushFlowStopped(FlowKey key, TimelinePoint point, Exception error)
		{
			_queue.Enqueue(_timelineDb.PushFlowStopped(key, point, error));
		}
	}
}