using System;
using System.Threading.Tasks;
using Xunit;

namespace Totem.Timeline.IntegrationTests
{
  /// <summary>
  /// Tests that scheduled events occur within 50ms of the due time
  /// </summary>
  public class ScheduledEvents : TestArea
  {
    [Fact]
    public async Task OccurWithin50Ms()
    {
      await Append(new StartTimer());

      await ExpectScheduled<TimerDue>();
      await Expect<TimerDue>(1500);

      var measured = await Expect<TimerMeasured>();

      Expect(measured.Lag.TotalMilliseconds).IsLessThanOrEqualTo(50);
    }

    class TestTopic : Topic
    {
      DateTimeOffset _whenOccurs;

      void GivenScheduled(TimerDue e) =>
        _whenOccurs = Event.GetWhenOccurs(e).Value;

      void When(StartTimer e) =>
        ThenSchedule.At(new TimerDue(), Clock.Now.AddSeconds(1));

      void When(TimerDue e) =>
        Then(new TimerMeasured(e.When - _whenOccurs));
    }

    class StartTimer : Command { }
    class TimerDue : Event { }

    class TimerMeasured : Event
    {
      public TimerMeasured(TimeSpan lag)
      {
        Lag = lag;
      }

      public readonly TimeSpan Lag;
    }
  }
}