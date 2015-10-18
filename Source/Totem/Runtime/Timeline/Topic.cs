using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A process observing the timeline and making decisions
	/// </summary>
	public abstract class Topic : Flow
	{
		[Transient] protected new TopicCall Call { get { return (TopicCall) base.Call; } }
		[Transient] protected new TopicType Type { get { return (TopicType) base.Type; } }

		protected void Then(Event e)
		{
			ExpectCallingWhen();

			Call.Append(e);
		}

		protected void Then(IEnumerable<Event> events)
		{
			ExpectCallingWhen();

			foreach(var e in events)
			{
				Call.Append(e);
			}
		}

		protected void Then(params Event[] events)
		{
			Then(events as IEnumerable<Event>);
		}

		protected void ThenAt(DateTime whenOccurs, Event e)
		{
			Message.Traits.When.Set(e, whenOccurs);

			Then(new EventScheduled(e));
		}

		protected void ThenAt(DateTime whenOccurs, IEnumerable<Event> events)
		{
			foreach(var e in events)
			{
				ThenAt(whenOccurs, e);
			}
		}

		protected void ThenAt(DateTime whenOccurs, params Event[] events)
		{
			ThenAt(whenOccurs, events as IEnumerable<Event>);
		}

		protected void ThenAt(TimeSpan timeOfDay, Event e)
		{
			ThenAt(GetWhenOccursNext(timeOfDay), e);
		}

		protected void ThenAt(TimeSpan timeOfDay, IEnumerable<Event> events)
		{
			ThenAt(GetWhenOccursNext(timeOfDay), events);
		}

		protected void ThenAt(TimeSpan timeOfDay, params Event[] events)
		{
			ThenAt(GetWhenOccursNext(timeOfDay), events);
		}

		private DateTime GetWhenOccursNext(TimeSpan timeOfDay)
		{
			// The time of day is relative to the timezone of the principal

			var now = Clock.Now.ToLocalTime();
			var today = now.Date;

			var whenToday = today + timeOfDay;

			var whenOccurs = whenToday > now
				? whenToday
				: today.AddDays(1) + timeOfDay;

			return whenOccurs.ToUniversalTime();
		}
	}
}