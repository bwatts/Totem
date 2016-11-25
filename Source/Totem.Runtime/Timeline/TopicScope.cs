using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Autofac;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of a topic's activity on the timeline
  /// </summary>
  internal sealed class TopicScope : FlowScope
  {
    readonly ILifetimeScope _lifetime;

    internal TopicScope(ILifetimeScope lifetime, TimelineScope timeline, FlowRoute initialRoute)
      : base(timeline, initialRoute)
    {
      _lifetime = lifetime;
    }

    protected override async Task PushPoint()
    {
      Log.Verbose("[timeline] {Position:l} => {Flow:l}", Point.Position, Key);

      try
      {
        await CallAndPushTopic();
      }
      catch(Exception error)
      {
        PushStopped(error);
      }
    }

    async Task CallAndPushTopic()
    {
      var topicEvent = GetTopicEvent();

      TryCallGiven(topicEvent);

      var newEvents = await TryCallWhen(topicEvent);

      var result = Timeline.PushTopic((Topic) Flow, Point, newEvents);

      if(result.GivenError || Flow.Context.Done)
      {
        CompleteTask();
      }
    }

    TopicEvent GetTopicEvent()
    {
      return (TopicEvent) Key.Type.Events.Get(Point.Event);
    }

    void TryCallGiven(TopicEvent topicEvent)
    {
      if(Point.Route.Given && !Point.Route.Then)
      {
        new FlowCall.Given(Point, topicEvent).Make((Topic) Flow);
      }
    }

    async Task<Many<Event>> TryCallWhen(TopicEvent topicEvent)
    {
      if(!Point.Route.When)
      {
        return new Many<Event>();
      }

      using(var scope = _lifetime.BeginCallScope())
      {
        var call = new FlowCall.TopicWhen(
          Point,
          topicEvent,
          scope.Resolve<IDependencySource>(),
          Timeline.ReadPrincipal(Point),
          State.CancellationToken);

        await call.Make(Flow);

        return call.RetrieveNewEvents();
      }
    }

    void PushStopped(Exception error)
    {
      Log.Error(error, "[timeline] [{Key:l}] Flow stopped", Key);

      try
      {
        Flow.Context.SetError(Point.Position);

        Timeline.PushStopped(Point, error);
      }
      finally
      {
        CompleteTask(error);
      }
    }
  }
}