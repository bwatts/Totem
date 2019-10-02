using System.Threading.Tasks;
using Totem.Timeline.IntegrationTests.Hosting;
using Xunit;

namespace Totem.Timeline.IntegrationTests
{
  /// <summary>
  /// Tests that multiple events in a When method get written after it completes
  /// </summary>
  public class MultipleWhenEvents : IntegrationTest
  {
    [Fact]
    public async Task WrittenAfterWhen()
    {
      await Append(new StartTest());

      await Expect<FirstHappened>();
      await Expect<SecondHappened>();
      await Expect<FirstObserved>();
      await Expect<SecondObserved>();
    }

    class TopicA : Topic
    {
      async Task When(StartTest e)
      {
        Then(new FirstHappened());

        await Task.Delay(100);

        Then(new SecondHappened());
      }
    }

    class TopicB : Topic
    {
      void When(FirstHappened e) =>
        Then(new FirstObserved());

      void When(SecondHappened e) =>
        Then(new SecondObserved());
    }

    class StartTest : Command { }
    class FirstHappened : Event { }
    class SecondHappened : Event { }
    class FirstObserved : Event { }
    class SecondObserved : Event { }
  }
}