using System;
using System.ComponentModel;
using System.Linq;
using Totem.IO;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// Identifies the type of a runtime object
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class MapTypeKey : IEquatable<MapTypeKey>, IComparable<MapTypeKey>
  {
    MapTypeKey(AreaKey area, string name)
    {
      Area = area;
      Name = name;
    }

    public readonly AreaKey Area;
    public readonly string Name;

    public override string ToString() => $"{Area}:{Name}";

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as MapTypeKey);

    public bool Equals(MapTypeKey other) =>
      Eq.Values(this, other).Check(x => x.Area).Check(x => x.Name);

    public override int GetHashCode() =>
      HashCode.Combine(Area, Name);

    public int CompareTo(MapTypeKey other) =>
      Cmp.Values(this, other).Check(x => x.Area).Check(x => x.Name);

    public static bool operator ==(MapTypeKey x, MapTypeKey y) => Eq.Op(x, y);
    public static bool operator !=(MapTypeKey x, MapTypeKey y) => Eq.OpNot(x, y);
    public static bool operator >(MapTypeKey x, MapTypeKey y) => Cmp.Op(x, y) > 0;
    public static bool operator <(MapTypeKey x, MapTypeKey y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(MapTypeKey x, MapTypeKey y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(MapTypeKey x, MapTypeKey y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    public static bool TryFrom(string value, out MapTypeKey key)
    {
      key = null;

      var parts = (value ?? "").Split(':');

      return parts.Length == 2
        && AreaKey.TryFrom(parts[0], out var areaKey)
        && TryFrom(areaKey, parts[1], out key);
    }

    public static bool TryFrom(string area, string name, out MapTypeKey key)
    {
      if(AreaKey.TryFrom(area, out var areaKey) && TryFrom(areaKey, name, out key))
      {
        return true;
      }

      key = null;

      return false;
    }

    public static bool TryFrom(AreaKey area, string name, out MapTypeKey key)
    {
      var nameParts = name.Split('.');

      key = nameParts.Length > 0 && nameParts.All(Id.IsName) ? new MapTypeKey(area, name) : null;

      return key != null;
    }

    public static MapTypeKey From(string value)
    {
      if(!TryFrom(value, out var key))
      {
        throw new FormatException($"Failed to parse map type key: {value}");
      }

      return key;
    }

    public static MapTypeKey From(string area, string name)
    {
      if(!TryFrom(area, name, out var key))
      {
        throw new FormatException($"Failed to parse map type key from area \"{area}\" and name \"{name}\"");
      }

      return key;
    }

    public static MapTypeKey From(AreaKey area, string name)
    {
      if(!TryFrom(area, name, out var key))
      {
        throw new FormatException($"Failed to parse map type key from area \"{area}\" and name \"{name}\"");
      }

      return key;
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}