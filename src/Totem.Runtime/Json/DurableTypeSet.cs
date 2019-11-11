using System;
using System.Collections.Generic;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// The durable types known to Totem JSON serialization
  /// </summary>
  public sealed class DurableTypeSet : IDurableTypeSet
  {
    readonly Dictionary<DurableTypeKey, DurableType> _byKey = new Dictionary<DurableTypeKey, DurableType>();
    readonly Dictionary<Type, DurableType> _byDeclaredType = new Dictionary<Type, DurableType>();

    public DurableTypeSet(IEnumerable<DurableType> knownTypes)
    {
      foreach(var knownType in knownTypes)
      {
        Add(knownType);
      }
    }

    public bool TryGetOrAdd(Type declaredType, out DurableType type)
    {
      lock(_byKey)
      {
        if(!_byDeclaredType.TryGetValue(declaredType, out type))
        {
          TryAdd(declaredType, out type);
        }
      }

      return type != null;
    }

    public bool TryGetKey(Type type, out DurableTypeKey key)
    {
      key = TryGetOrAdd(type, out var durableType) ? durableType.Key : null;

      return key != null;
    }

    public bool TryGetByKey(DurableTypeKey key, out Type type)
    {
      lock(_byKey)
      {
        type = _byKey.TryGetValue(key, out var durableType) ? durableType.DeclaredType : null;

        return type != null;
      }
    }

    void Add(DurableType type)
    {
      _byKey[type.Key] = type;
      _byDeclaredType[type.DeclaredType] = type;
    }

    void TryAdd(Type declaredType, out DurableType type)
    {
      if(DurableType.TryFrom(declaredType, out type))
      {
        Add(type);
      }
    }
  }
}