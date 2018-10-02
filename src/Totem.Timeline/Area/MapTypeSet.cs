using System;
using System.Collections;
using System.Collections.Generic;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of types in a runtime area
  /// </summary>
  /// <typeparam name="T">The type of runtime types in the set</typeparam>
  public class MapTypeSet<T> : IEnumerable<T> where T : MapType
  {
    readonly Dictionary<MapTypeKey, T> _typesByKey = new Dictionary<MapTypeKey, T>();
    readonly Dictionary<Type, T> _typesByDeclaredType = new Dictionary<Type, T>();

    public int Count => _typesByKey.Count;
    public T this[MapTypeKey key] => _typesByKey[key];
    public IEnumerable<MapTypeKey> Keys => _typesByKey.Keys;
    public IEnumerable<T> Values => _typesByKey.Values;
    public IEnumerable<KeyValuePair<MapTypeKey, T>> Pairs => _typesByKey;

    public bool Contains(MapTypeKey key) => _typesByKey.ContainsKey(key);

    public T Get(MapTypeKey key, bool strict = true)
    {
      if(!_typesByKey.TryGetValue(key, out var type) && strict)
      {
        throw new Exception($"Unknown domain key: {key}");
      }

      return type;
    }

    public IEnumerator<T> GetEnumerator() => _typesByKey.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //
    // Declared types
    //

    public IEnumerable<Type> DeclaredTypes => _typesByDeclaredType.Keys;
    public IEnumerable<KeyValuePair<Type, T>> DeclaredTypePairs => _typesByDeclaredType;
    public T this[Type declaredType] => _typesByDeclaredType[declaredType];

    public bool Contains(Type declaredType) => _typesByDeclaredType.ContainsKey(declaredType);

    public T Get(Type declaredType, bool strict = true)
    {
      if(!_typesByDeclaredType.TryGetValue(declaredType, out var type) && strict)
      {
        throw new KeyNotFoundException($"Unknown declared type: {declaredType}");
      }

      return type;
    }

    protected internal void DeclareIfNotAlready(T type)
    {
      if(!Contains(type.Key))
      {
        Declare(type);
      }
    }

    protected internal void Declare(T type)
    {
      DeclareByKey(type);

      DeclareByOtherKeys(type);
    }

    void DeclareByKey(T type)
    {
      if(Contains(type.Key))
      {
        throw new Exception(Text.None
          .WriteLine("The key {0} is associated with multiple types", type.Key)
          .WriteLine()
          .WriteLine(_typesByKey[type.Key])
          .Write(type));
      }

      _typesByKey.Add(type.Key, type);

      if(Contains(type.DeclaredType))
      {
        throw new Exception(Text.None
          .WriteLine("The declared type {0} is associated with multiple types", type.DeclaredType)
          .WriteLine()
          .WriteLine(_typesByDeclaredType[type.DeclaredType])
          .Write(type));
      }

      _typesByDeclaredType.Add(type.DeclaredType, type);
    }

    internal virtual void DeclareByOtherKeys(T type)
    {}
  }
}