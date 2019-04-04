using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Totem.Timeline.IntegrationTests
{
  /// <summary>
  /// Tests that routing to a multi-instance flow creates all specified instances
  /// </summary>
  public class RoutingToMultiInstance : TestArea
  {
    [Fact]
    public async Task CreatesAllInstances()
    {
      var ids = Many.Of(Id.From("A"), Id.From("B"), Id.From("C"));

      await Append(new StartTest(ids));

      var created = Many.Of(
        await Expect<InstanceCreated>(),
        await Expect<InstanceCreated>(),
        await Expect<InstanceCreated>());

      Expect(created
        .Select(e => e.Id)
        .OrderBy(id => id)
        .SequenceEqual(ids));
    }

    class TestTopic : Topic
    {
      static Many<Id> RouteFirst(StartTest e) => e.Ids;

      void When(StartTest e) =>
        Then(new InstanceCreated(Id));
    }

    class StartTest : Event
    {
      public StartTest(Many<Id> ids)
      {
        Ids = ids;
      }

      public readonly Many<Id> Ids;
    }

    class InstanceCreated : Event
    {
      public InstanceCreated(Id id)
      {
        Id = id;
      }

      public readonly Id Id;
    }
  }
}