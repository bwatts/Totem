using System;
using System.Collections;
using System.Collections.Generic;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of events observed within a <see cref="Flow"/>, indexed by key and declared type
  /// </summary>
  public class FlowObservationSet : IReadOnlyCollection<FlowObservation>
  {
    readonly Dictionary<MapTypeKey, FlowObservation> _byKey = new Dictionary<MapTypeKey, FlowObservation>();
    readonly Dictionary<Type, FlowObservation> _byDeclaredType = new Dictionary<Type, FlowObservation>();

    public FlowObservationSet()
    {}

    public FlowObservationSet(IEnumerable<FlowObservation> observations)
    {
      foreach(var observation in observations)
      {
        Declare(observation);
      }
    }

    public int Count => _byKey.Count;
    public FlowObservation this[MapTypeKey key] => _byKey[key];
    public FlowObservation this[EventType type] => _byKey[type.Key];
    public FlowObservation this[Type declaredType] => _byDeclaredType[declaredType];
    public FlowObservation this[Event e] => _byDeclaredType[e.GetType()];

    public IEnumerator<FlowObservation> GetEnumerator() => _byKey.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Contains(MapTypeKey key) =>
      _byKey.ContainsKey(key);

    public bool Contains(EventType e) =>
      Contains(e.DeclaredType);

    public bool Contains(Type declaredType) =>
      _byDeclaredType.ContainsKey(declaredType);

    public bool Contains(Event e) =>
      Contains(e.GetType());

    public bool TryGet(MapTypeKey key, out FlowObservation observation) =>
      _byKey.TryGetValue(key, out observation);

    public bool TryGet(EventType type, out FlowObservation observation) =>
      TryGet(type.DeclaredType, out observation);

    public bool TryGet(Type declaredType, out FlowObservation observation) =>
      _byDeclaredType.TryGetValue(declaredType, out observation);

    public bool TryGet(Event e, out FlowObservation observation) =>
      TryGet(e.GetType(), out observation);

    public FlowObservation Get(MapTypeKey key)
    {
      if(!TryGet(key, out var observation))
      {
        throw new KeyNotFoundException($"This set does not contain the specified key: {key}");
      }

      return observation;
    }

    public FlowObservation Get(EventType type)
    {
      if(!TryGet(type, out var observation))
      {
        throw new KeyNotFoundException($"This set does not contain the specified event: {type}");
      }

      return observation;
    }

    public FlowObservation Get(Type declaredType)
    {
      if(!TryGet(declaredType, out var observation))
      {
        throw new KeyNotFoundException($"This set does not contain the specified event: {declaredType}");
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
      _byKey[observation.EventType.Key] = observation;
      _byDeclaredType[observation.EventType.DeclaredType] = observation;
    }
  }
}