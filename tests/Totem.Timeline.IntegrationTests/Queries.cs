using System.Threading.Tasks;
using Totem.Timeline.IntegrationTests.Hosting;
using Xunit;

namespace Totem.Timeline.IntegrationTests
{
  /// <summary>
  /// Tests that query state updates after each Given method
  /// </summary>
  public class Queries : IntegrationTest
  {
    [Fact]
    public async Task StateUpdatesAfterGiven()
    {
      var initial = await GetQuery<TestQuery>();

      await Append(new Added("DEF"));

      var afterAdded = await GetQuery<TestQuery>();

      await Append(new Added("GHI"));

      var afterAddedAgain = await GetQuery<TestQuery>();

      await Append(new Replaced("123"));

      var afterReplaced = await GetQuery<TestQuery>();

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