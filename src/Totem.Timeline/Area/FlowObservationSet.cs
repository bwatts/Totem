using System;
using System.Collections;
using System.Collections.Generic;
using Totem.Reflection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of events observed within a <see cref="Flow"/>, indexed by name and declared type
  /// </summary>
  public class FlowObservationSet : IReadOnlyCollection<FlowObservation>
  {
    readonly Dictionary<TypeName, FlowObservation> _byName = new Dictionary<TypeName, FlowObservation>();
    readonly Dictionary<Type, FlowObservation> _byDeclaredType = new Dictionary<Type, FlowObservation>();

    internal FlowObservationSet()
    {}

    internal FlowObservationSet(IEnumerable<FlowObservation> observations)
    {
      foreach(var observation in observations)
      {
        Declare(observation);
      }
    }

    public int Count => _byName.Count;
    public FlowObservation this[TypeName name] => _byName[name];
    public FlowObservation this[EventType type] => _byName[type.Name];
    public FlowObservation this[Type declaredType] => _byDeclaredType[declaredType];
    public FlowObservation this[Event e] => _byDeclaredType[e.GetType()];

    public IEnumerator<FlowObservation> GetEnumerator() => _byName.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Contains(TypeName name) =>
      _byName.ContainsKey(name);

    public bool Contains(EventType e) =>
      Contains(e.DeclaredType);

    public bool Contains(Type declaredType) =>
      _byDeclaredType.ContainsKey(declaredType);

    public bool Contains(Event e) =>
      Contains(e.GetType());

    public bool TryGet(TypeName name, out FlowObservation observation) =>
      _byName.TryGetValue(name, out observation);

    public bool TryGet(EventType type, out FlowObservation observation) =>
      TryGet(type.DeclaredType, out observation);

    public bool TryGet(Type declaredType, out FlowObservation observation) =>
      _byDeclaredType.TryGetValue(declaredType, out observation);

    public bool TryGet(Event e, out FlowObservation observation) =>
      TryGet(e.GetType(), out observation);

    public FlowObservation Get(TypeName name)
    {
      if(!TryGet(name, out var observation))
      {
        throw new KeyNotFoundException($"This set does not contain the specified name: {name}");
      }

      return observation;
    }

    public FlowObservation Get(EventType type)
    {
      if(!TryGet(type, out var observation))
      {
        throw new KeyNotFoundException($"This set does not contain the specified event type: {type}");
      }

      return observation;
    }

    public FlowObservation Get(Type declaredType)
    {
      if(!TryGet(declaredType, out var observation))
      {
        throw new KeyNotFoundException($"This set does not contain the specified event type: {declaredType}");
      }

      return observation;
    }

    public FlowObservation Get(Event e)
    {
      if(!TryGet(e, out var observation))
      {
        throw new KeyNotFoundException($"This set does not contain the specified event: {e.GetType()}");
      }

      return observation;
    }

    internal void Declare(FlowObservation observation)
    {
      _byName[observation.EventType.Name] = observation;
      _byDeclaredType[observation.EventType.DeclaredType] = observation;
    }
  }
}