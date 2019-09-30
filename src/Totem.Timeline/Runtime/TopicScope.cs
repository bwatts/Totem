using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime;
using Totem.Timeline.Area;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// The scope of a topic's activity on the timeline
  /// </summary>
  public class TopicScope : FlowScope<Topic>
  {
    readonly IServiceProvider _services;

    public TopicScope(FlowKey key, ITimelineDb db, IServiceProvider services) : base(key, db)
    {
      _services = services;
    }

    new TopicObservation Observation => (TopicObservation) base.Observation;
    bool CanCallWhen => Observation.HasWhen(Point.Scheduled);
    bool CanCallGiven => Observation.HasGiven(Point.Scheduled);
    bool GivenWasNotImmediate => Point.Topic != Key;

    protected override async Task ObservePoint()
    {
      if(CanCallWhen)
      {
        LogPoint();

        if(CanCallGiven && GivenWasNotImmediate)
        {
          CallGivenBeforeWhen();
        }

        await CallWhenAndWrite();
      }
      else
      {
        if(GivenWasNotImmediate)
        {
          LogPoint();

          CallGivenWithoutWhen();

          await WriteCheckpoint();
        }
      }
    }

    void LogPoint() =>
      LogPoint(Point);

    void LogPoint(TimelinePoint point) =>
      Log.Trace("[timeline] #{Position} => {Key}", point.Position.ToInt64(), Key);

    void CallGivenBeforeWhen() =>
      Flow.Context.CallGiven(new FlowCall.Given(Point, Observation));

    void CallGivenWithoutWhen() =>
      Flow.Context.CallGiven(new FlowCall.Given(Point, Observation), advanceCheckpoint: true);

    //
    // When
    //

    async Task CallWhenAndWrite()
    {
      var newEvents = await CallWhen();

      if(newEvents.Count == 0)
      {
        await WriteCheckpoint();
      }
      else
      {
        var newPosition = await Db.WriteNewEvents(Point.Position, Key, newEvents);

        var immediateGivens = GetImmediateGivens(newEvents, newPosition).ToMany();

        if(immediateGivens.Count == 0)
        {
          await WriteCheckpointAfterNewEvents(Point);
        }
        else
        {
          await CallImmediateGivensAndWrite(immediateGivens);
        }
      }
    }

    async Task<Many<Event>> CallWhen()
    {
      using(var callScope = _services.CreateScope())
      {
        var call = new FlowCall.When(Point, Observation, callScope.ServiceProvider, State.CancellationToken);

        await Flow.Context.CallWhen(call);

        return call.GetNewEvents();
      }
    }

    IEnumerable<FlowCall.Given> GetImmediateGivens(Many<Event> newEvents, TimelinePosition newPosition)
    {
      foreach(var e in newEvents)
      {
        if(TryGetImmediateGiven(e, newPosition, out var given))
        {
          yield return given;
        }

        newPosition = newPosition.Next();
      }
    }

    bool TryGetImmediateGiven(Event e, TimelinePosition position, out FlowCall.Given given)
    {
      given = null;

      if(TryGetObservation(e, out var observation))
      {
        var routes = observation.EventType.GetRoutes(e).ToMany();

        if(routes.Contains(Key))
        {
          var point = new TimelinePoint(
            position,
            Point.Position,
            observation.EventType,
            e.When,
            Event.Traits.WhenOccurs.Get(e),
            Event.Traits.EventId.Get(e),
            Event.Traits.CommandId.Get(e),
            Event.Traits.UserId.Get(e),
            Key,
            routes,
            () => e);

          given = new FlowCall.Given(point, observation);
        }
      }

      return given != null;
    }

    bool TryGetObservation(Event e, out FlowObservation observation) =>
      Key.Type.Observations.TryGet(e, out observation)
      && observation.HasGiven(Event.IsScheduled(e));

    //
    // After writing new events
    //

    async Task WriteCheckpointAfterNewEvents(TimelinePoint point)
    {
      try
      {
        await WriteCheckpoint();
      }
      catch(Exception error)
      {
        await StopAfterNewEvents(point, error);
      }
    }

    async Task CallImmediateGivensAndWrite(Many<FlowCall.Given> givens)
    {
      var latestPoint = Point;
      var advanceCheckpoint = true;

      try
      {
        foreach(var given in givens)
        {
          latestPoint = given.Point;

          LogPoint(latestPoint);

          var observation = (TopicObservation) given.Observation;

          var hasWhen = observation.HasWhen(latestPoint.Scheduled);

          advanceCheckpoint = advanceCheckpoint && !hasWhen;

          Flow.Context.CallGiven(given, advanceCheckpoint);
        }
      }
      catch(Exception error)
      {
        await StopAfterNewEvents(latestPoint, error);

        return;
      }

      await WriteCheckpointAfterNewEvents(latestPoint);
    }

    async Task StopAfterNewEvents(TimelinePoint latestPoint, Exception error)
    {
      try
      {
        Flow.Context.SetError(latestPoint.Position, error.ToString());

        await Db.WriteCheckpoint(Flow, latestPoint);

        Flow.Context.SetNotNew();

        CompleteTask(error);
      }
      catch(Exception writeError)
      {
        CompleteTask(new Exception(
          $"Topic {Key} added events to the timeline, but failed to save its checkpoint. This should be vanishingly rare and would be surprising if it occurred. This can be reconciled when resuming via each new point's topic key, but that is not in place yet, so the flow is stopped. Manual resolution is required.",
          new AggregateException(error, writeError)));
      }
    }
  }
}