using System.Threading.Tasks;
using Xunit;

namespace Totem.Timeline.IntegrationTests
{
  /// <summary>
  /// Tests that Given methods occur before When methods for the same event
  /// </summary>
  public class GivenWhenOrder : TestArea
  {
    [Fact]
    public async Task GivenBeforeWhenForSameEvent()
    {
      await Append(new Increment());

      var incremented = await Expect<Incremented>();

      await Append(new Increment());

      var incrementedAgain = await Expect<Incremented>();

      Expect(incremented.Value).Is(1);
      Expect(incrementedAgain.Value).Is(2);
    }

    class TestTopic : Topic
    {
      int _value;

      void Given(Increment e) =>
        _value++;

      void When(Increment e) =>
        Then(new Incremented(_value));
    }

    class Increment : Command { }

    class Incremented : Event
    {
      public Incremented(int value)
      {
        Value = value;
      }

      public readonly int Value;
    }
  }
}