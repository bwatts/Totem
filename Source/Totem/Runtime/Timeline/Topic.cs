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

		protected void ThenDone(Event e)
		{
			Then(e);

			ThenDone();
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

		protected void ThenScheduleInterval(Event e, TimeSpan interval)
		{
			ThenScheduleInterval(e, interval, TimeSpan.Zero);
		}

		protected void ThenScheduleInterval(Event e, TimeSpan interval, TimeSpan offset)
		{
			ThenSchedule(e, GetWhenIntervalOccursNext(interval, offset));
		}

		// The time of day is relative to the timezone of the principal

		private DateTime GetWhenOccursNext(TimeSpan timeOfDay)
		{
			var now = Clock.Now.ToLocalTime();
			var today = now.Date;

			var whenToday = today + timeOfDay;

			var whenOccurs = whenToday > now
				? whenToday
				: today.AddDays(1) + timeOfDay;

			return whenOccurs.ToUniversalTime();
		}

		private DateTime GetWhenIntervalOccursNext(TimeSpan interval, TimeSpan offset)
		{
			Expect(interval).IsGreaterThan(TimeSpan.Zero, "Interval must indicate a time in the future.");

			var now = Clock.Now.ToLocalTime();
			var today = now.Date;

			var whenOccursNext = today + offset;

			while(whenOccursNext < now)
			{
				whenOccursNext += interval;
			}

			var whenOccurs = whenOccursNext.Date == now.Date ? whenOccursNext : now.Date.AddDays(1) + offset;

			return whenOccurs.ToUniversalTime();
		}
	}
}