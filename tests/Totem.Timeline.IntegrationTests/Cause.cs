using System.Threading.Tasks;
using Totem.Timeline.IntegrationTests.Hosting;
using Xunit;

namespace Totem.Timeline.IntegrationTests
{
  /// <summary>
  /// Tests that the cause of an event is set by the When method
  /// </summary>
  public class Cause : IntegrationTest
  {
    [Fact]
    public async Task SetByWhen()
    {
      await Append(new StartTest());

      await Expect<TestStarted>();

      var observed = await Expect<TestStartedObserved>();

      Expect(observed.Cause).Is(new TimelinePosition(0));
      Expect(observed.Position).Is(new TimelinePosition(1));
    }

    class TestTopic : Topic
    {
      void When(StartTest e) =>
        Then(new TestStarted());

      void When(TestStarted e) =>
        Then(new TestStartedObserved
        {
          Cause = Context.Call.Point.Cause,
          Position = Context.Call.Point.Position
        });
    }

    class StartTest : Command { }
    class TestStarted : Event { }

    class TestStartedObserved : Event
    {
      public TimelinePosition Cause;
      public TimelinePosition Position;
    }
  }
}