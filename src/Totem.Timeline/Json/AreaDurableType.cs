using System;
using Totem.Runtime.Json;
using Totem.Timeline.Area;

namespace Totem.Timeline.Json
{
  /// <summary>
  /// A timeline area type known to Totem JSON serialization
  /// </summary>
  internal sealed class AreaDurableType : IDurableType
  {
    readonly DurableType _type;

    internal AreaDurableType(DurableType type)
    {
      _type = type;
    }

    public string Key => _type.Key.ToString();
    public Type DeclaredType => _type.DeclaredType;

    public object Create() =>
      _type.CreateToDeserialize();
  }
}