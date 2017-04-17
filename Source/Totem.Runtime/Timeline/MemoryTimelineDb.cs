using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Totem.Runtime.Json;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A database persisting events and flows in runtime memory
	/// </summary>
	public sealed class MemoryTimelineDb : Notion, ITimelineDb, IViewDb
	{
		readonly Dictionary<FlowKey, Flow> _flowsByKey = new Dictionary<FlowKey, Flow>();
		long _position = -1;

    public Task<ResumeInfo> ReadResumeInfo()
		{
      return Task.FromResult(new ResumeInfo());
		}

		public Task<Flow> ReadFlow(FlowRoute route, bool strict = true)
		{
      Flow flow;

      if(!_flowsByKey.TryGetValue(route.Key, out flow))
      {
        ExpectNot(strict, "Flow not found: " + route.Key.ToText());
      }

      return Task.FromResult(flow);
		}

		public Task<TimelineMessage> Push(TimelinePosition cause, Event e)
		{
      return Task.FromResult(PushNext(new PendingPoint(cause, e)));
		}

		public Task<TimelineMessage> PushScheduled(TimelinePoint point)
		{
      return Push(point.Position, point.Event);
		}

		public Task<TimelineMessage> PushStopped(FlowPoint point, Exception error)
		{
			var stopped = new FlowStopped(
        point.Route.Key.Type.Key,
        point.Route.Key.Id,
        error.ToString());

			Flow.Traits.BindRequestId(point.Event, stopped);
      Flow.Traits.BindUserId(point.Event, stopped);

      return Push(point.Position, stopped);
		}

    public async Task<PushTopicResult> PushTopic(Topic topic, FlowPoint point, IEnumerable<Event> newEvents)
    {
      var newPoints =
        from newEvent in newEvents
        select new PendingPoint(topic.Context.Key, point.Position, newEvent);

      var messages = new Many<TimelineMessage>();
      var givenError = false;

      foreach(var newPoint in newPoints)
      {
        var newMessage = PushNext(newPoint);

        messages.Write.Add(newMessage);

        if(newPoint.HasThenRoute
          && !givenError
          && !(await CallGiven(topic, newPoint, newMessage)))
        {
          givenError = true;
        }
      }

      if(givenError || topic.Context.Done)
      {
        lock(_flowsByKey)
        {
          _flowsByKey.Remove(topic.Context.Key);
        }
      }

      return new PushTopicResult(messages, givenError);
    }

    public Task PushView(View view)
    {
      return Task.CompletedTask;
    }

    TimelineMessage PushNext(PendingPoint point)
		{
			lock(_flowsByKey)
			{
				var message = PushNewPoint(point);

				if(point.Scheduled)
				{
					Log.Info(
            "[timeline] {Cause:l} ++ {Position:l} >> {EventType:l} @ {When}",
            point.Cause,
            message.Point.Position,
            point.ScheduledEventType,
            point.ScheduledEvent.When);
				}
				else if(point.HasTopicKey)
				{
					Log.Info("[timeline] {Cause:l} ++ {Point:l} << {Topic:l}", point.Cause, message.Point, point.TopicKey);
				}
				else
				{
					Log.Info("[timeline] {Cause:l} ++ {Point:l}", point.Cause, message.Point);
				}

        return message;
			}
		}

    async Task<bool> CallGiven(Topic topic, PendingPoint newPoint, TimelineMessage newMessage)
    {
      var thenPoint = new FlowPoint(newPoint.ThenRoute, newMessage.Point);

      try
      {
        var topicEvent = (TopicEvent) topic.Context.Type.Events.Get(newMessage.Point.EventType);

        var call = new FlowCall.Given(thenPoint, topicEvent);

        call.Make(topic);

        return true;
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] Flow {Flow:l} stopped", topic.Context.Key);

        await PushStopped(thenPoint, error);

        return false;
      }
    }

    TimelineMessage PushNewPoint(PendingPoint point)
		{
			var message = point.ToMessage(IncrementPosition());

			InitializeNewFlows(message);

			return message;
		}

		TimelinePosition IncrementPosition()
		{
			return new TimelinePosition(Interlocked.Increment(ref _position));
		}

		void InitializeNewFlows(TimelineMessage message)
		{
			foreach(var route in message.Routes)
			{
				if(!_flowsByKey.ContainsKey(route.Key) && (route.First || route.Key.Type.IsSingleInstance))
				{
					var flow = route.Key.Type.New();

          FlowContext.Bind(flow, route.Key);

					_flowsByKey[route.Key] = flow;
				}
			}
		}

		//
		// Views
		//

		public Task<ViewSnapshot<string>> ReadJsonSnapshot(Type type, Id id, TimelinePosition checkpoint)
		{
      return Task.FromResult(ReadView(type, id, checkpoint, view => JsonFormat.Text.Serialize(view).ToString()));
		}

    public Task<ViewSnapshot<View>> ReadSnapshot(Type type, Id id, TimelinePosition checkpoint)
		{
			return Task.FromResult(ReadView(type, id, checkpoint, view => (View) view));
		}

		public Task<ViewSnapshot<T>> ReadSnapshot<T>(Id id, TimelinePosition checkpoint) where T : View
		{
      return Task.FromResult(ReadView(typeof(T), id, checkpoint, view => (T) view));
		}

		ViewSnapshot<T> ReadView<T>(Type type, Id id, TimelinePosition checkpoint, Func<Flow, T> selectContent)
		{
			var key = Runtime.GetView(type).CreateKey(id);

			Flow view;

			if(!_flowsByKey.TryGetValue(key, out view))
			{
				return ViewSnapshot<T>.OfNotFound(key);
			}
			else if(view.Context.CheckpointPosition == checkpoint)
			{
				return ViewSnapshot<T>.OfNotModified(key, checkpoint);
			}
			else
			{
				return ViewSnapshot<T>.OfContent(key, view.Context.CheckpointPosition, selectContent(view));
			}
		}
	}
}