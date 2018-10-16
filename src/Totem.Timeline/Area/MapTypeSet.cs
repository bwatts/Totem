using System;
using System.Collections;
using System.Collections.Generic;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of .NET types representing types in a timeline map
  /// </summary>
  /// <typeparam name="T">The type of map types in the set</typeparam>
  public class MapTypeSet<T> : IEnumerable<T> where T : MapType
  {
    readonly Dictionary<MapTypeKey, T> _byKey = new Dictionary<MapTypeKey, T>();
    readonly Dictionary<Type, T> _byDeclaredType = new Dictionary<Type, T>();

    public MapTypeSet(IEnumerable<T> types)
    {
      foreach(var type in types)
      {
        _byKey[type.Key] = type;
        _byDeclaredType[type.DeclaredType] = type;
      }
    }

    public int Count => _byKey.Count;
    public T this[MapTypeKey key] => Get(key);
    public IEnumerable<MapTypeKey> Keys => _byKey.Keys;
    public IEnumerable<T> Types => _byKey.Values;
    public IEnumerable<KeyValuePair<MapTypeKey, T>> TypesByKey => _byKey;

    public IEnumerator<T> GetEnumerator() => Types.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Contains(MapTypeKey key) =>
      _byKey.ContainsKey(key);

    public bool TryGet(MapTypeKey key, out T type) =>
      _byKey.TryGetValue(key, out type);

    public T Get(MapTypeKey key)
    {
      if(!TryGet(key, out var type))
      {
        throw new KeyNotFoundException($"This set does not contain the specified key: {key}");
      }

      return type;
    }

    //
    // Declared types
    //

    public T this[Type declaredType] => Get(declaredType);
    public IEnumerable<Type> DeclaredTypes => _byDeclaredType.Keys;
    public IEnumerable<KeyValuePair<Type, T>> TypesByDeclaredType => _byDeclaredType;

    public bool Contains(Type declaredType) =>
      _byDeclaredType.ContainsKey(declaredType);

    public bool TryGet(Type declaredType, out T type) =>
      _byDeclaredType.TryGetValue(declaredType, out type);

    public T Get(Type declaredType)
    {
      if(!TryGet(declaredType, out var type))
      {
        throw new KeyNotFoundException($"This set does not contain the specified type: {declaredType}");
      }

      return type;
    }
  }
}