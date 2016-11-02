using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of a topic's activity on the timeline
  /// </summary>
  public sealed class TopicScope : FlowScope<Topic>
  {
    public TopicScope(ILifetimeScope lifetime, TimelineScope timeline, Topic instance)
      : base(lifetime, timeline, instance)
    {}

    protected override async Task PushPoint()
    {
      try
      {
        var topicEvent = (TopicEvent) GetFlowEvent();

        TryCallGiven(topicEvent);

        await CallWhen(topicEvent);

        CompleteTaskIfDone();
      }
      catch(Exception error)
      {
        PushStopped(error);
      }
    }

    void TryCallGiven(TopicEvent topicEvent)
    {
      if(Point.Route.Given && !Point.Route.Then)
      {
        new FlowCall.Given(Point, topicEvent).Make(Instance);
      }
    }

    async Task CallWhen(TopicEvent topicEvent)
    {
      using(var scope = BeginCallScope())
      {
        var call = new FlowCall.TopicWhen(
          Point,
          topicEvent,
          scope.Resolve<IDependencySource>(),
          ReadPrincipal(),
          State.CancellationToken);

        await call.Make(Instance);

        var result = PushWhen(call);

        if(result.GivenError)
        {
          CompleteTask();
        }
      }
    }
  }
}