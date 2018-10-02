using System;
using System.Collections.Generic;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of runtime objects indexed by key
  /// </summary>
  /// <typeparam name="TKey">The type of key identifying runtime objects in the set</typeparam>
  /// <typeparam name="TValue">The type of keyed runtime objects in the set</typeparam>
  public abstract class MapSet<TKey, TValue>
  {
    readonly Dictionary<TKey, TValue> _set = new Dictionary<TKey, TValue>();

    public int Count => _set.Count;
    public TValue this[TKey key] => _set[key];
    public IEnumerable<TKey> Keys => _set.Keys;
    public IEnumerable<TValue> Values => _set.Values;
    public IEnumerable<KeyValuePair<TKey, TValue>> Pairs => _set;

    public bool Contains(TKey key) =>
      _set.ContainsKey(key);

    public TValue Get(TKey key, bool strict = true)
    {
      if(!_set.TryGetValue(key, out var value) && strict)
      {
        throw new KeyNotFoundException($"Unknown key: {key}");
      }

      return value;
    }

    internal void DeclareIfNotAlready(TValue value)
    {
      var key = GetKey(value);

      if(!Contains(key))
      {
        Declare(key, value);
      }
    }

    internal void Declare(TValue value) =>
      Declare(GetKey(value), value);

    void Declare(TKey key, TValue value)
    {
      DeclareByKey(key, value);

      DeclareByOtherKeys(value);
    }

    void DeclareByKey(TKey key, TValue value)
    {
      if(Contains(key))
      {
        throw new Exception(Text.None
          .WriteLine("The key {0} is associated with multiple values", key)
          .WriteLine()
          .WriteLine(_set[key])
          .Write(value));
      }

      _set.Add(key, value);
    }

    internal abstract TKey GetKey(TValue value);

    internal virtual void DeclareByOtherKeys(TValue value)
    {}
  }
}