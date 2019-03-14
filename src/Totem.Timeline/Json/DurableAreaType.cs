using System;
using Totem.Runtime.Json;
using Totem.Timeline.Area;

namespace Totem.Timeline.Json
{
  /// <summary>
  /// A type in a timeline area known to Totem JSON serialization
  /// </summary>
  internal sealed class DurableAreaType : IDurableType
  {
    readonly AreaType _type;

    internal DurableAreaType(AreaType type)
    {
      _type = type;
    }

    public string Key => _type.Name.ToString();
    public Type DeclaredType => _type.DeclaredType;

    public object Create() => _type.CreateToDeserialize();
  }
}