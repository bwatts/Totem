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

    public static MapTypeKey From(string value, bool strict = true)
    {
      var areaParts = (value ?? "").Split(':');

      if(areaParts.Length == 2)
      {
        var area = AreaKey.From(areaParts[0], strict);

        if(area != null)
        {
          return From(area, areaParts[1], strict);
        }
      }

      Expect.False(strict, $"Failed to parse runtime key: {value}");

      return null;
    }

    public static MapTypeKey From(string area, string name, bool strict = true)
    {
      var parsedArea = AreaKey.From(area, strict);

      return parsedArea == null ? null : From(parsedArea, name, strict);
    }

    public static MapTypeKey From(AreaKey area, string name, bool strict = true)
    {
      var nameParts = name.Split('.');

      if(nameParts.Length > 0 && nameParts.All(Id.IsName))
      {
        return new MapTypeKey(area, name);
      }

      Expect.False(strict, $"Failed to parse runtime key name: {name}");

      return null;
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) => From(value);
    }
  }
}