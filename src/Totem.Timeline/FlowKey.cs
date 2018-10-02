using System;
using Totem.Timeline.Area;

namespace Totem.Timeline
{
  /// <summary>
  /// References a flow instance by type and optional identifier
  /// </summary>
  public sealed class FlowKey : IEquatable<FlowKey>, IComparable<FlowKey>
  {
    FlowKey(FlowType type, Id id)
    {
      Type = type;
      Id = id;
    }

    public readonly FlowType Type;
    public readonly Id Id;

    public override string ToString() =>
      Text.Of(Type).WriteIf(Id.IsAssigned, $"{Id.Separator}{Id}");

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as FlowKey);

    public bool Equals(FlowKey other) =>
      Eq.Values(this, other).Check(x => x.Type).Check(x => x.Id);

    public override int GetHashCode() =>
      HashCode.Combine(Type, Id);

    public int CompareTo(FlowKey other) =>
      Cmp.Values(this, other).Check(x => x.Type).Check(x => x.Id);

    public static bool operator ==(FlowKey x, FlowKey y) => Eq.Op(x, y);
    public static bool operator !=(FlowKey x, FlowKey y) => Eq.OpNot(x, y);
    public static bool operator >(FlowKey x, FlowKey y) => Cmp.Op(x, y) > 0;
    public static bool operator <(FlowKey x, FlowKey y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(FlowKey x, FlowKey y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(FlowKey x, FlowKey y) => Cmp.Op(x, y) <= 0;

    public static FlowKey From(FlowType type) =>
      From(type, Id.Unassigned);

    public static FlowKey From(FlowType type, Id id) =>
      new FlowKey(type, id);

    public static FlowKey From(MapTypeSet<FlowType> flows, string value, bool strict = true)
    {
      var idIndex = value.IndexOf(Id.Separator);

      var typePart = idIndex == -1 ? value : value.Substring(0, idIndex);
      var idPart = idIndex == -1 ? "" : value.Substring(idIndex + 1);

      var typeKey = MapTypeKey.From(typePart, strict);

      if(typeKey != null)
      {
        var type = flows.Get(typeKey, strict);

        if(type != null)
        {
          return new FlowKey(type, Id.From(idPart));
        }
      }

      Expect.False(strict, $"Failed to parse flow key: {value}");

      return null;
    }

    public static FlowKey From(AreaMap area, string value, bool strict = true) =>
      From(area.Flows, value, strict);
  }
}