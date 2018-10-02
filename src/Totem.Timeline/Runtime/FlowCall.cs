using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Totem.Runtime;
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
    /// A call to a .When method of a <see cref="Flow"/>
    /// </summary>
    public class When : FlowCall
    {
      public When(
        TimelinePoint point,
        FlowObservation observation,
        IServiceProvider services,
        CancellationToken cancellationToken)
        : base(point, observation)
      {
        Services = services;
        CancellationToken = cancellationToken;
      }

      public readonly IServiceProvider Services;
      public readonly CancellationToken CancellationToken;

      public Task Make(Flow flow) =>
        Observation.CallWhen(flow, this);
    }

    /// <summary>
    /// A call to a .When method of a <see cref="Topic"/>
    /// </summary>
    public sealed class TopicWhen : When
    {
      readonly ConcurrentQueue<Event> _newEvents;
      bool _retrieved;

      public TopicWhen(
        TimelinePoint point,
        TopicObservation observation,
        IServiceProvider services,
        CancellationToken cancellationToken)
        : base(point, observation, services, cancellationToken)
      {
        _newEvents = new ConcurrentQueue<Event>();
      }

      public void Append(Event e)
      {
        Event.Traits.CommandId.Bind(Point.Event, e);
        Event.Traits.UserId.Bind(Point.Event, e);

        _newEvents.Enqueue(e);
      }

      public Many<Event> RetrieveNewEvents()
      {
        Expect.False(_retrieved, "New events already retrieved");

        _retrieved = true;

        return _newEvents.ToMany();
      }
    }

    /// <summary>
    /// A call to a .Given method of a <see cref="Topic"/>
    /// </summary>
    public sealed class Given : FlowCall
    {
      public Given(TimelinePoint point, TopicObservation observation) : base(point, observation)
      {}

      public new TopicObservation Observation =>
        (TopicObservation) base.Observation;

      public void Make(Topic topic) =>
        Observation.CallGiven(topic, this);
    }
  }
}