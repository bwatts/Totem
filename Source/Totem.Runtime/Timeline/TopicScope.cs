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
    readonly Subject<FlowPoint> _points = new Subject<FlowPoint>();
    readonly ILifetimeScope _lifetime;
    readonly TimelineScope _timeline;
    FlowRoute _initialRoute;
    Topic _topic;

    internal TopicScope(ILifetimeScope lifetime, TimelineScope timeline, FlowRoute initialRoute)
      : base(initialRoute.Key)
    {
      _lifetime = lifetime;
      _timeline = timeline;
      _initialRoute = initialRoute;
    }

    protected override void Enqueue(FlowPoint point)
    {
      _points.OnNext(point);
    }

    protected override void Open()
    {
      Track(_points
        .ObserveOn(ThreadPoolScheduler.Instance)
        .SelectMany(OnPoint)
        .Subscribe());
    }

    async Task<Unit> OnPoint(FlowPoint point)
    {
      Point = point;

      if(TopicLoaded())
      {
        await PushPoint();
      }

      return default(Unit);
    }

    //
    // Load
    //

    bool TopicLoaded()
    {
      if(_topic == null && _initialRoute != null)
      {
        LoadTopic();

        _initialRoute = null;
      }

      return _topic != null;
    }

    void LoadTopic()
    {
      try
      {
        TryLoadTopic();
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] [{Key:l}] Failed to load topic", Key);

        CompleteTask(error);
      }
    }

    void TryLoadTopic()
    {
      Log.Verbose("[timeline] [{Key:l}] Loading...", Key);

      Flow flow;

      if(!_timeline.TryReadFlow(_initialRoute, out flow))
      {
        Log.Verbose("[timeline] [{Key:l}] Routed topic does not yet exist; ignoring", Key);

        CompleteTask();
      }
      else if(flow.Context.HasError)
      {
        Log.Verbose("[timeline] [{Key:l}] Topic is stopped; ignoring", Key);

        CompleteTask(new Exception($"Topic {Key} is stopped"));
      }
      else
      {
        _topic = (Topic) flow;
      }
    }

    //
    // Point
    //

    async Task PushPoint()
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

      var result = _timeline.PushTopic(_topic, Point, newEvents);

      if(result.GivenError || _topic.Context.Done)
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
        new FlowCall.Given(Point, topicEvent).Make(_topic);
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
          _timeline.ReadPrincipal(Point),
          State.CancellationToken);

        await call.Make(_topic);

        return call.RetrieveNewEvents();
      }
    }

    void PushStopped(Exception error)
    {
      Log.Error(error, "[timeline] [{Key:l}] Flow stopped", Key);

      try
      {
        _topic.Context.SetError(Point.Position);

        _timeline.PushStopped(Point, error);
      }
      finally
      {
        CompleteTask(error);
      }
    }
  }
}