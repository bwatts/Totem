using System;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// Describes the durable types known to Totem JSON serialization
  /// </summary>
  public interface IDurableTypeSet
  {
    bool TryGetOrAdd(Type declaredType, out DurableType type);

    bool TryGetKey(Type declaredType, out DurableTypeKey key);

    bool TryGetByKey(DurableTypeKey key, out Type declaredType);
  }
}