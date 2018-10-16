using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Totem.IO;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// Identifies a timeline area using C# identifier rules
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class AreaKey : IEquatable<AreaKey>, IComparable<AreaKey>
  {
    readonly string _name;

    AreaKey(string name)
    {
      _name = name;
    }

    public override string ToString() => _name;

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as AreaKey);

    public bool Equals(AreaKey other) =>
      Eq.Values(this, other).Check(x => x._name);

    public override int GetHashCode() =>
      _name.GetHashCode();

    public int CompareTo(AreaKey other) =>
      Cmp.Values(this, other).Check(x => x._name);

    public static bool operator ==(AreaKey x, AreaKey y) => Eq.Op(x, y);
    public static bool operator !=(AreaKey x, AreaKey y) => Eq.OpNot(x, y);
    public static bool operator >(AreaKey x, AreaKey y) => Cmp.Op(x, y) > 0;
    public static bool operator <(AreaKey x, AreaKey y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(AreaKey x, AreaKey y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(AreaKey x, AreaKey y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    static readonly Regex _regex = new Regex(
      "^"               // Start the string
      + "[A-Za-z_]"     // Match C# identifier rules for the first character
      + "[A-Za-z_0-9]*" // Match C# identifier rules for remaining characters
      + "$",            // End the string
      RegexOptions.Compiled);

    public static bool TryFrom(string value, out AreaKey key)
    {
      key = Id.IsName(value) ? new AreaKey(value) : null;

      return key != null;
    }

    public static AreaKey From(string value)
    {
      if(!TryFrom(value, out var key))
      {
        throw new FormatException($"Failed to parse area key: {value}");
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