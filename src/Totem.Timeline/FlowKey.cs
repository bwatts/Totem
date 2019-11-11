using System;
using Totem.Reflection;
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
      type.ExpectIdMatchesCardinality(id);

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

    public static bool TryFrom(string value, AreaMap area, out FlowKey key)
    {
      var idIndex = value.IndexOf(Id.Separator);

      var typePart = idIndex == -1 ? value : value.Substring(0, idIndex);
      var idPart = idIndex == -1 ? "" : value.Substring(idIndex + 1);

      key = TypeName.TryFrom(typePart, out var typeKey) && area.TryGetFlow(typeKey, out var type)
        ? new FlowKey(type, Id.From(idPart))
        : null;

      return key != null;
    }

    public static FlowKey From(FlowType type, Id id) =>
      new FlowKey(type, id);

    public static FlowKey From(FlowType type) =>
      new FlowKey(type, Id.Unassigned);

    public static FlowKey From(string value, AreaMap area)
    {
      if(!TryFrom(value, area, out var key))
      {
        throw new FormatException($"Failed to parse the specified key: {value}");
      }

      return key;
    }
  }
}