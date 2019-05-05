using System;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// Describes a durable type known to Totem JSON serialization
  /// </summary>
  public interface IDurableType
  {
    string Key { get; }

    Type DeclaredType { get; }

    object Create();
  }
}