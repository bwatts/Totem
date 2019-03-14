using System.Collections.Generic;
using System.Linq;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type declaring an event on the timeline
  /// </summary>
  public sealed class EventType : AreaType
  {
    internal EventType(AreaTypeInfo info, Many<FlowObservation> observations = null) : base(info)
    {
      Observations = observations ?? new Many<FlowObservation>();
    }

    public readonly Many<FlowObservation> Observations;

    public IEnumerable<FlowKey> GetRoutes(Event e, bool scheduled) =>
      Observations.SelectMany(observation => observation.GetRoutes(e, scheduled));

    public IEnumerable<FlowKey> GetRoutes(Event e) =>
      GetRoutes(e, Event.IsScheduled(e));
  }
}