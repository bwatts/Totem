using System.Threading.Tasks;
using Xunit;

namespace Totem.Timeline.IntegrationTests
{
  /// <summary>
  /// Tests that query state updates after each Given method
  /// </summary>
  public class Queries : TestArea
  {
    [Fact]
    public async Task StateUpdatesAfterGiven()
    {
      var initial = await Get<TestQuery>();
      var afterAdded = await GetAfter<TestQuery>(new Added("DEF"));
      var afterAddedAgain = await GetAfter<TestQuery>(new Added("GHI"));
      var afterReplaced = await GetAfter<TestQuery>(new Replaced("123"));

      Expect(initial.Value).Is("ABC");
      Expect(afterAdded.Value).Is("ABCDEF");
      Expect(afterAddedAgain.Value).Is("ABCDEFGHI");
      Expect(afterReplaced.Value).Is("123");
    }

    class TestQuery : Query
    {
      public string Value = "ABC";

      void Given(Added e) => Value += e.Value;
      void Given(Replaced e) => Value = e.Value;
    }

    class Added : Event
    {
      public Added(string value)
      {
        Value = value;
      }

      public readonly string Value;
    }

    class Replaced : Event
    {
      public Replaced(string value)
      {
        Value = value;
      }

      public readonly string Value;
    }
  }
}