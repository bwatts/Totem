using System.Threading.Tasks;
using Xunit;

namespace Totem.Timeline.IntegrationTests
{
  /// <summary>
  /// Tests that routing to a multi-instance flow creates all specified instances
  /// </summary>
  public class RoutingToLatentInstance : TestArea
  {
    [Fact]
    public async Task OnlyCreatesForRouteFirst()
    {
      var id = Id.From("A");

      await Append(new FirstHappened(id));
      await Append(new SecondHappened(id));

      await Expect<SecondObserved>();

      await Append(new FirstHappened(id));

      await Expect<FirstObserved>();
    }

    class TestTopic : Topic
    {
      static Id Route(FirstHappened e) => e.Id;
      static Id RouteFirst(SecondHappened e) => e.Id;

      void When(FirstHappened e) =>
        Then(new FirstObserved(Id));

      void When(SecondHappened e) =>
        Then(new SecondObserved(Id));
    }

    class FirstHappened : Event
    {
      public FirstHappened(Id id)
      {
        Id = id;
      }

      public readonly Id Id;
    }

    class FirstObserved : Event
    {
      public FirstObserved(Id id)
      {
        Id = id;
      }

      public readonly Id Id;
    }

    class SecondHappened : Event
    {
      public SecondHappened(Id id)
      {
        Id = id;
      }

      public readonly Id Id;
    }

    class SecondObserved : Event
    {
      public SecondObserved(Id id)
      {
        Id = id;
      }

      public readonly Id Id;
    }
  }
}