using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Timeline
{
  /// <summary>
  /// Extends <see cref="ITopicScheduler"/> with various scheduling algorithms
  /// </summary>
  public static class TopicScheduler
  {
    public static void NextTimeOfDay(this ITopicScheduler scheduler, Event e, TimeSpan timeOfDay) =>
      scheduler.At(e, scheduler.GetNextTimeOfDay(timeOfDay));

    public static void NextTimeOfDay(this ITopicScheduler scheduler, Event e, params TimeSpan[] timesOfDay) =>
      scheduler.NextTimeOfDay(e, timesOfDay as IEnumerable<TimeSpan>);

    public static void NextTimeOfDay(this ITopicScheduler scheduler, Event e, IEnumerable<TimeSpan> timesOfDay)
    {
      var whenOccurs = timesOfDay
        .Select(scheduler.GetNextTimeOfDay)
        .DefaultIfEmpty(DateTimeOffset.MinValue)
        .Min();

      Expect.True(whenOccurs > DateTimeOffset.MinValue, "Scheduling requires at least one time of day");

      scheduler.At(e, whenOccurs);
    }

    static DateTimeOffset GetNextTimeOfDay(this ITopicScheduler scheduler, TimeSpan timeOfDay)
    {
      var now = scheduler.Now;
      var today = now.Date;

      var whenToday = today + timeOfDay;

      return whenToday > now ? whenToday : today.AddDays(1) + timeOfDay;
    }

    //
    // Interval
    //

    public static void NextInterval(this ITopicScheduler scheduler, Event e, TimeSpan interval) =>
      scheduler.NextInterval(e, interval, TimeSpan.Zero);

    public static void NextInterval(this ITopicScheduler scheduler, Event e, TimeSpan interval, TimeSpan offset)
    {
      Expect.That(interval).IsGreaterThan(TimeSpan.Zero, "Scheduling requires an interval in the future");

      var now = scheduler.Now;
      var today = now.Date;

      var whenOccursNext = today + offset;

      while(whenOccursNext < now)
      {
        whenOccursNext += interval;
      }

      var whenOccurs = whenOccursNext.Date == now.Date ? whenOccursNext : now.Date.AddDays(1) + offset;

      scheduler.At(e, whenOccurs);
    }
  }
}