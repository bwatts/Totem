using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// A timeline presence that makes decisions
  /// </summary>
  public abstract class Topic : Flow
	{
		[Transient] public new TopicType Type => (TopicType) base.Type;
    [Transient] protected new TopicWhenCall WhenCall => (TopicWhenCall) base.WhenCall;

		protected void Then(Event e)
		{
			ExpectCallingWhen();

			WhenCall.Append(e);
		}

		protected void ThenSchedule(Event e, DateTime whenOccurs)
		{
			Message.Traits.When.Set(e, whenOccurs);

			Then(new EventScheduled(e));
		}

		protected void ThenSchedule(Event e, TimeSpan timeOfDay)
		{
			ThenSchedule(e, GetWhenOccursNext(timeOfDay));
		}

		protected void ThenSchedule(Event e, IEnumerable<TimeSpan> timesOfDay)
		{
			ThenSchedule(e, timesOfDay.Select(timeOfDay => GetWhenOccursNext(timeOfDay)).Min());
		}

		protected void ThenSchedule(Event e, params TimeSpan[] timesOfDay)
		{
			ThenSchedule(e, timesOfDay as IEnumerable<TimeSpan>);
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