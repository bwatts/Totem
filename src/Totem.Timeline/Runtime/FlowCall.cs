using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Totem.Timeline.Area;

namespace Totem.Timeline
{
  /// <summary>
  /// A call to a method defined by a <see cref="Flow"/>
  /// </summary>
  public abstract class FlowCall
  {
    internal FlowCall(TimelinePoint point, FlowObservation observation)
    {
      Point = point;
      Observation = observation;
    }

    public readonly TimelinePoint Point;
    public readonly FlowObservation Observation;

    /// <summary>
    /// A call to a .Given method of a <see cref="Flow"/>
    /// </summary>
    public sealed class Given : FlowCall
    {
      public Given(TimelinePoint point, FlowObservation observation) : base(point, observation)
      {}

      public void Make(Flow flow) =>
        Observation.CallGiven(flow, this);
    }

    /// <summary>
    /// A call to a .When method of a <see cref="Topic"/>
    /// </summary>
    public sealed class When : FlowCall
    {
      readonly ConcurrentQueue<Event> _newEvents = new ConcurrentQueue<Event>();

      public When(
        TimelinePoint point,
        TopicObservation observation,
        IServiceProvider services,
        CancellationToken cancellationToken)
        : base(point, observation)
      {
        Services = services;
        CancellationToken = cancellationToken;
      }

      public readonly IServiceProvider Services;
      public readonly CancellationToken CancellationToken;

      public new TopicObservation Observation =>
        (TopicObservation) base.Observation;

      public Task Make(Topic topic) =>
        Observation.CallWhen(topic, this);

      public void Append(Event e)
      {
        Event.Traits.CommandId.Bind(Point.Event, e);
        Event.Traits.UserId.Bind(Point.Event, e);

        _newEvents.Enqueue(e);
      }

      public Many<Event> GetNewEvents() =>
        _newEvents.ToMany();
    }
  }
}