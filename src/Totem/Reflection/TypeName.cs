using System;
using System.ComponentModel;
using System.Linq;
using Totem.IO;

namespace Totem.Reflection
{
  /// <summary>
  /// The single name of a type or multiple names of a nested type
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class TypeName : IEquatable<TypeName>, IComparable<TypeName>
  {
    readonly string _value;

    TypeName(string value)
    {
      _value = value;
    }

    public override string ToString() => _value;

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as TypeName);

    public bool Equals(TypeName other) =>
      Eq.Values(this, other).Check(x => x._value);

    public override int GetHashCode() =>
      _value.GetHashCode();

    public int CompareTo(TypeName other) =>
      Cmp.Values(this, other).Check(x => x._value);

    public static bool operator ==(TypeName x, TypeName y) => Eq.Op(x, y);
    public static bool operator !=(TypeName x, TypeName y) => Eq.OpNot(x, y);
    public static bool operator >(TypeName x, TypeName y) => Cmp.Op(x, y) > 0;
    public static bool operator <(TypeName x, TypeName y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(TypeName x, TypeName y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(TypeName x, TypeName y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    public static bool TryFrom(string value, out TypeName name)
    {
      var parts = (value ?? "").Split('.');

      name = parts.Length > 0 && parts.All(Id.IsName) ? new TypeName(value) : null;

      return name != null;
    }

    public static TypeName From(string value)
    {
      if(!TryFrom(value, out var name))
      {
        throw new FormatException($"Failed to parse type name: {value}. Expected one or more names, separated by '.', following C# identifier rules.");
      }

      return name;
    }

    public static TypeName From(Type type)
    {
      var currentType = type;
      var currentName = type.Name;

      while(currentType.IsNested)
      {
        currentType = currentType.DeclaringType;
        currentName = $"{currentType.Name}.{currentName}";
      }

      return new TypeName(currentName);
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}