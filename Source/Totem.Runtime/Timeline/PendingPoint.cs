using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A new event pending a push to the timeline
	/// </summary>
	public sealed class PendingPoint : Notion
	{
		public PendingPoint(FlowKey topicKey, TimelinePosition cause, Event e)
		{
			TopicKey = topicKey;
			HasTopicKey = topicKey != null;

			Cause = cause;
			Event = e;
			EventType = Runtime.GetEvent(e.GetType());

			var scheduled = e as EventScheduled;

			if(scheduled != null)
			{
				Scheduled = true;
				ScheduledEvent = scheduled.Event;
				ScheduledEventType = Runtime.GetEvent(ScheduledEvent.GetType());

				Routes = Route(ScheduledEventType, ScheduledEvent);
			}
			else
			{
				Routes = Route(EventType, Event);
			}

      ThenRoute = Routes.FirstOrDefault(route => route.Then);
      HasThenRoute = ThenRoute != null;
		}

		public PendingPoint(TimelinePosition cause, Event e) : this(null, cause, e)
		{}

		public readonly FlowKey TopicKey;
		public readonly bool HasTopicKey;
		public readonly TimelinePosition Cause;
		public readonly Event Event;
		public readonly EventType EventType;
		public readonly bool Scheduled;
		public readonly Event ScheduledEvent;
		public readonly EventType ScheduledEventType;
		public readonly Many<FlowRoute> Routes;
    public readonly FlowRoute ThenRoute;
    public readonly bool HasThenRoute;

		private Many<FlowRoute> Route(EventType type, Event e)
		{
			var whenRoutes = type.RouteWhen(e, Scheduled).ToList();
			var givenRoutes = type.RouteGiven(e, Scheduled).ToList();

			var flowKeys = whenRoutes.Concat(givenRoutes).Select(route => route.Key).Distinct();

			return Many.Of(
				from flowKey in flowKeys
				join whenRoute in whenRoutes on flowKey equals whenRoute.Key into flowWhens
				join givenRoute in givenRoutes on flowKey equals givenRoute.Key into flowGivens
				from flowWhen in flowWhens.DefaultIfEmpty()
				from flowGiven in flowGivens.DefaultIfEmpty()
				let when = flowWhen != null
				let given = flowGiven != null
				let first = (when && flowWhen.First) || (given && flowGiven.First)
				let then = given && HasTopicKey && flowKey == TopicKey
				select new FlowRoute(flowKey, first, when, given, then));
		}

		public TimelineMessage ToMessage(TimelinePosition position)
		{
			var newPoint = new TimelinePoint(
				position,
				Cause,
				Scheduled ? ScheduledEventType : EventType,
				Scheduled ? ScheduledEvent : Event,
				Scheduled);

			return new TimelineMessage(newPoint, Routes);
		}
	}
}