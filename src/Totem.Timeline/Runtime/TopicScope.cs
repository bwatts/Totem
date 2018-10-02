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
    readonly AreaMap _area;

    public TopicScope(FlowKey key, IServiceProvider services, ITimelineDb db, AreaMap area)
      : base(key, services, db)
    {
      _area = area;
    }

    protected new TopicObservation Observation => (TopicObservation) base.Observation;

    protected override async Task ObservePoint()
    {
      if(CanCallWhen)
      {
        LogPoint();

        using(var callScope = Services.CreateScope())
        {
          CallGiven();

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

    bool CanCallWhen =>
      Observation.HasWhen(Point.Scheduled);

    void LogPoint() =>
      Log.Debug("[timeline] #{Position} => {Key}", Point.Position.ToInt64(), Key);

    void CallGiven() =>
      Flow.Context.CallGiven(new FlowCall.Given(Point, Observation));

    void CallGivenWithoutWhen() =>
      Flow.Context.CallGiven(new FlowCall.Given(Point, Observation), advanceCheckpoint: true);

    bool GivenWasNotImmediate =>
      Point.Topic != Key;

    async Task CallWhen(IServiceProvider services)
    {
      var call = new FlowCall.TopicWhen(Point, Observation, services, State.CancellationToken);

      await Flow.Context.CallWhen(call);

      var newEvents = call.RetrieveNewEvents();

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
        foreach(var call in immediateGivens.Calls)
        {
          Flow.Context.CallGiven(call, advanceCheckpoint: !call.Observation.HasWhen());
        }
      }
      catch(Exception error)
      {
        await Stop(error);
      }

      try
      {
        await WriteCheckpoint();
      }
      catch(Exception error)
      {
        await Stop(new Exception(
          $"Topic {Key} added events to the timeline, but failed to save its checkpoint. This should be vanishingly rare and would be surprising if it occurred. This can be reconciled when resuming via AreaEventMetadata.Topic, but that is not in place yet, so the flow is stopped. Manual resolution is required.",
          error));
      }
    }
  }
}