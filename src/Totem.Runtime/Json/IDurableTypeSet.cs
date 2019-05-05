using System;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// Describes the durable types known to Totem JSON serialization
  /// </summary>
  public interface IDurableTypeSet
  {
    bool Contains(Type type);

    bool TryGetOrAdd(Type type, out Func<object> create);

    bool TryGetKey(Type type, out string key);

    bool TryGetByKey(string key, out Type type);
  }
}