using System;
using System.Collections.Generic;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// The durable types known to Totem JSON serialization
  /// </summary>
  public sealed class DurableTypeSet : IDurableTypeSet
  {
    readonly Dictionary<string, IDurableType> _byKey = new Dictionary<string, IDurableType>();
    readonly Dictionary<Type, IDurableType> _byDeclaredType = new Dictionary<Type, IDurableType>();

    public DurableTypeSet(IEnumerable<IDurableType> types)
    {
      foreach(var type in types)
      {
        _byKey[type.Key] = type;
        _byDeclaredType[type.DeclaredType] = type;
      }
    }

    public bool Contains(Type type) =>
      _byDeclaredType.ContainsKey(type);

    public bool TryGetByKey(string key, out Type type)
    {
      type = _byKey.TryGetValue(key, out var durable) ? durable.DeclaredType : null;

      return type != null;
    }

    public bool TryGetKey(Type type, out string key)
    {
      key = _byDeclaredType.TryGetValue(type, out var durable) ? durable.Key : null;

      return key != null;
    }

    public bool TryGetOrAdd(Type type, out Func<object> create)
    {
      create = _byDeclaredType.TryGetValue(type, out var durable)
        ? durable.Create
        : null as Func<object>;

      return create != null;
    }
  }
}