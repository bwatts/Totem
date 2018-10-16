using System.Collections.Generic;
using System.Linq;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type representing an event on the timeline
  /// </summary>
  public class EventType : MapType
  {
    public EventType(MapTypeInfo type, Many<FlowObservation> observations = null) : base(type)
    {
      Observations = observations ?? new Many<FlowObservation>();
    }

    public readonly Many<FlowObservation> Observations;

    public IEnumerable<FlowKey> GetRoutes(Event e, bool scheduled) =>
      Observations.SelectMany(observation => observation.GetRoutes(e, scheduled));

    public IEnumerable<FlowKey> GetRoutes(Event e) =>
      GetRoutes(e, Event.Traits.IsScheduled(e));
  }
}