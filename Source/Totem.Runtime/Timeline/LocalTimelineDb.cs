using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Totem.Runtime.Json;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A database persisting timeline data in the local runtime
	/// </summary>
	public sealed class LocalTimelineDb : Notion, ITimelineDb, IViewDb
	{
		private readonly Dictionary<FlowKey, Flow> _flowsByKey = new Dictionary<FlowKey, Flow>();
		private long _position = -1;

		public ResumeInfo ReadResumeInfo()
		{
			return new ResumeInfo(new TimelinePosition(0));
		}

		public bool TryReadFlow(FlowRoute route, out Flow flow)
		{
			return _flowsByKey.TryGetValue(route.Key, out flow);
		}

		public TimelineMessage Push(TimelinePosition cause, Event e)
		{
			return PushNext(new PendingPoint(cause, e));
		}

		public TimelineMessage PushScheduled(TimelineMessage message)
		{
			return Push(message.Point.Position, message.Point.Event);
		}

		public TimelineMessage PushStopped(FlowPoint point, Exception error)
		{
			var stopped = new FlowStopped(
				point.Route.Key.Type.Key,
				point.Route.Key.Id,
				error.ToString());

			Flow.Traits.ForwardRequestId(point.Event, stopped);

			return Push(point.Position, stopped);
		}

    public PushWhenResult PushWhen(Flow flow, FlowCall.When call)
    {
      lock(_flowsByKey)
      {
        var topicCall = call as FlowCall.TopicWhen;

        var result = topicCall != null
          ? PushTopicWhen((Topic) flow, topicCall)
          : new PushWhenResult();

        if(result.FlowStopped || flow.Context.Done)
        {
          _flowsByKey.Remove(flow.Context.Key);
        }

        return result;
      }
    }

    //
    // PushNext
    //

    private TimelineMessage PushNext(PendingPoint point)
		{
			lock(_flowsByKey)
			{
				var message = PushNewPoint(point);

				if(point.Scheduled)
				{
					Log.Info("[timeline] {Cause:l} ++ {Position:l} >> {EventType:l} @ {When}", point.Cause, message.Point.Position, point.EventType, point.ScheduledEvent.When);
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

		private Many<TimelineMessage> PushNext(IEnumerable<PendingPoint> points)
		{
			lock(_flowsByKey)
			{
				return points.ToMany(PushNext);
			}
		}

    //
    // Push topic when
    //

		private PushWhenResult PushTopicWhen(Topic topic, FlowCall.TopicWhen call)
		{
      var newPoints = call
        .RetrieveNewEvents()
        .ToMany(e => new PendingPoint(topic.Context.Key, call.Point.Position, e));

      var messages = new Many<TimelineMessage>();
      var flowStopped = false;

			lock(_flowsByKey)
			{
				foreach(var newPoint in newPoints)
				{
					var message = PushNext(newPoint);

          messages.Write.Add(message);

          if(newPoint.HasThenRoute
            && !flowStopped
            && !CallGiven(topic, newPoint, message))
          {
            flowStopped = true;
          }
				}
			}

      return new PushWhenResult(messages, flowStopped);
    }

    private bool CallGiven(Topic topic, PendingPoint newPoint, TimelineMessage message)
    {
      var thenPoint = new FlowPoint(newPoint.ThenRoute, message.Point);

      try
      {
        var topicEvent = (TopicEvent) topic.Context.Type.Events.Get(message.Point.EventType);

        var call = new FlowCall.Given(thenPoint, topicEvent);

        call.Make(topic);

        return true;
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] Flow {Flow:l} stopped", topic.Context.Key);

        PushStopped(thenPoint, error);

        return false;
      }
    }

    //
    // Push new point
    //

    private TimelineMessage PushNewPoint(PendingPoint point)
		{
			var message = point.ToMessage(IncrementPosition());

			InitializeNewFlows(message);

			return message;
		}

		private TimelinePosition IncrementPosition()
		{
			return new TimelinePosition(Interlocked.Increment(ref _position));
		}

		private void InitializeNewFlows(TimelineMessage message)
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

		public ViewSnapshot<string> ReadJsonSnapshot(Type type, Id id, TimelinePosition checkpoint)
		{
			return ReadView(type, id, checkpoint, view => JsonFormat.Text.Serialize(view).ToString());
		}

		public ViewSnapshot<View> ReadSnapshot(Type type, Id id, TimelinePosition checkpoint)
		{
			return ReadView(type, id, checkpoint, view => (View) view);
		}

		public ViewSnapshot<T> ReadSnapshot<T>(Id id, TimelinePosition checkpoint) where T : View
		{
			return ReadView(typeof(T), id, checkpoint, view => (T) view);
		}

		private ViewSnapshot<T> ReadView<T>(Type type, Id id, TimelinePosition checkpoint, Func<Flow, T> selectContent)
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