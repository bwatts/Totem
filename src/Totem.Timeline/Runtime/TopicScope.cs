using System;
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

    public TopicScope(FlowKey key, ITimelineDb db, IServiceProvider services)
      : base(key, db)
    {
      _services = services;
    }

    protected new TopicObservation Observation =>
      (TopicObservation) base.Observation;

    protected override async Task ObservePoint()
    {
      if(CanCallWhen)
      {
        LogPoint();

        if(CanCallGiven)
        {
          CallGiven();
        }

        using(var callScope = _services.CreateScope())
        {
          await CallWhen(callScope.ServiceProvider);
        }
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

    bool CanCallWhen => Observation.HasWhen(Point.Scheduled);
    bool CanCallGiven => Observation.HasGiven(Point.Scheduled);
    bool GivenWasNotImmediate => Point.Topic != Key;

    void LogPoint() =>
      Log.Debug("[timeline] #{Position} => {Key}", Point.Position.ToInt64(), Key);

    void CallGiven() =>
      Flow.Context.CallGiven(new FlowCall.Given(Point, Observation));

    void CallGivenWithoutWhen() =>
      Flow.Context.CallGiven(new FlowCall.Given(Point, Observation), advanceCheckpoint: true);

    async Task CallWhen(IServiceProvider services)
    {
      var call = new FlowCall.When(Point, Observation, services, State.CancellationToken);

      await Flow.Context.CallWhen(call);

      var newEvents = call.GetNewEvents();

      if(newEvents.Count == 0)
      {
        await WriteCheckpoint();
      }
      else
      {
        await WriteAndCallImmediateGivens(newEvents);
      }
    }

    async Task WriteAndCallImmediateGivens(Many<Event> newEvents)
    {
      var immediateGivens = await Db.WriteNewEvents(Point.Position, Key, newEvents);

      try
      {
        CallImmediateGivens(immediateGivens);

        await WriteCheckpoint();
      }
      catch(Exception error)
      {
        await Stop(new Exception(
          $"Topic {Key} added events to the timeline, but failed to save its checkpoint. This should be vanishingly rare and would be surprising if it occurred. This can be reconciled when resuming via AreaEventMetadata.Topic, but that is not in place yet, so the flow is stopped. Manual resolution is required.",
          error));
      }
    }

    void CallImmediateGivens(ImmediateGivens immediateGivens)
    {
      foreach(var call in immediateGivens.Calls)
      {
        var observation = (TopicObservation) call.Observation;

        var hasWhen = observation.HasWhen(scheduled: true) || observation.HasWhen(scheduled: false);

        Flow.Context.CallGiven(call, advanceCheckpoint: !hasWhen);
      }
    }
  }
}