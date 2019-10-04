using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Totem.IO;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// Identifies a type in a timeline area
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class AreaTypeName : IEquatable<AreaTypeName>, IComparable<AreaTypeName>
  {
    readonly string _name;

    AreaTypeName(string name)
    {
      _name = name;
    }

    public override string ToString() => _name;

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as AreaTypeName);

    public bool Equals(AreaTypeName other) =>
      Eq.Values(this, other).Check(x => x._name);

    public override int GetHashCode() =>
      _name.GetHashCode();

    public int CompareTo(AreaTypeName other) =>
      Cmp.Values(this, other).Check(x => x._name);

    public static bool operator ==(AreaTypeName x, AreaTypeName y) => Eq.Op(x, y);
    public static bool operator !=(AreaTypeName x, AreaTypeName y) => Eq.OpNot(x, y);
    public static bool operator >(AreaTypeName x, AreaTypeName y) => Cmp.Op(x, y) > 0;
    public static bool operator <(AreaTypeName x, AreaTypeName y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(AreaTypeName x, AreaTypeName y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(AreaTypeName x, AreaTypeName y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    public static bool TryFrom(string value, out AreaTypeName name)
    {
      if (value.Contains("\"")) 
      {
                var span = value.AsSpan();
                var builder = new StringBuilder();
                for (int i = 0; i < span.Length; i++)
                {
                    if (span[i] != '"')
                      builder.Append(span[i]);
                }
                value = builder.ToString();
      }
      var parts = value.Split('.');
      
      

      name = parts.Length > 0 && parts.All(Id.IsName) ? new AreaTypeName(value) : null;

      return name != null;
    }

    public static AreaTypeName From(string value)
    {
      if(!TryFrom(value, out var name))
      {
        throw new FormatException($"Failed to parse area type name: {value}");
      }

      return name;
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}