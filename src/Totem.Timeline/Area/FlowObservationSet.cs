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
    readonly Dictionary<MapTypeKey, FlowObservation> _eventsByKey = new Dictionary<MapTypeKey, FlowObservation>();
    readonly Dictionary<Type, FlowObservation> _eventsByType = new Dictionary<Type, FlowObservation>();

    public IEnumerator<FlowObservation> GetEnumerator() => _eventsByKey.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _eventsByKey.Count;

    public bool Contains(EventType e) => Contains(e.DeclaredType);
    public bool Contains(MapTypeKey key) => _eventsByKey.ContainsKey(key);
    public bool Contains(Type declaredType) => _eventsByType.ContainsKey(declaredType);
    public bool Contains(Event e) => Contains(e.GetType());

    public FlowObservation Get(MapTypeKey key, bool strict = true)
    {
      if(_eventsByKey.TryGetValue(key, out var e))
      {
        return e;
      }

      Expect.False(strict, "Unknown event key: " + Text.Of(key));

      return null;
    }

    public FlowObservation Get(Type type, bool strict = true)
    {
      if(_eventsByType.TryGetValue(type, out var e))
      {
        return e;
      }

      Expect.False(strict, "Unknown event type: " + Text.Of(type));

      return null;
    }

    public FlowObservation Get(EventType type, bool strict = true) =>
      Get(type.DeclaredType, strict);

    public FlowObservation Get(Event e, bool strict = true) =>
      Get(e.GetType(), strict);

    internal void Declare(FlowObservation e)
    {
      if(_eventsByKey.TryGetValue(e.EventType.Key, out var current) && current != e)
      {
        throw new Exception($"Event {e.EventType} is already declared");
      }

      _eventsByKey.Add(e.EventType.Key, e);
      _eventsByType.Add(e.EventType.DeclaredType, e);
    }
  }
}