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
			ThenSchedule(e, timesOfDay.Select(GetWhenOccursNext).Min());
		}

		protected void ThenSchedule(Event e, params TimeSpan[] timesOfDay)
		{
			ThenSchedule(e, timesOfDay as IEnumerable<TimeSpan>);
		}

        protected void ThenScheduleRepeating(Event e, TimeSpan interval)
        {
            ThenScheduleRepeating(e, interval, TimeSpan.FromMinutes(0));
        }

        protected void ThenScheduleRepeating(Event e, TimeSpan interval, TimeSpan offset)
        {
            ThenSchedule(e, GetIntervalRange(interval, offset));
        }

        private IEnumerable<TimeSpan> GetIntervalRange(TimeSpan interval, TimeSpan offset)
        {
            //ensure offset starts as < 1 day 
            var nextRun = offset - TimeSpan.FromDays(offset.Days);

            while (nextRun < TimeSpan.FromDays(1))
            {
                yield return nextRun;
                nextRun += interval;
            }
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