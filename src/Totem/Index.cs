using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Totem
{
  /// <summary>
  /// The position of an item in a list
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public struct Index : IEquatable<Index>, IComparable<Index>
  {
    readonly int _value;

    Index(int value)
    {
      _value = value;
    }

    public int ToInt32() => _value;
    public override string ToString() => _value.ToString();

    public bool IsInRange(int count) =>
      count >= 0 && _value < count;

    public bool IsOutOfRange(int count) =>
      count >= 0 && _value >= count;

    public bool IsInRange(IList list) =>
      IsInRange(list.Count);

    public bool IsOutOfRange(IList list) =>
      IsOutOfRange(list.Count);

    public Index Max(Index other) =>
      _value <= other._value ? this : other;

    public void MoveTo(Index toIndex, IList list)
    {
      if(toIndex != this)
      {
        var item = list[_value];

        list.RemoveAt(_value);
        list.Insert(toIndex._value, item);
      }
    }

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      obj is Index index && Equals(index);

    public bool Equals(Index other) =>
      _value == other._value;

    public override int GetHashCode() =>
      _value;

    public int CompareTo(Index other) =>
      _value.CompareTo(other._value);

    public static bool operator ==(Index x, Index y) => Eq.Op(x, y);
    public static bool operator !=(Index x, Index y) => Eq.OpNot(x, y);
    public static bool operator >(Index x, Index y) => Cmp.Op(x, y) > 0;
    public static bool operator <(Index x, Index y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(Index x, Index y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(Index x, Index y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    public static explicit operator Index(int value) => From(value);
    public static implicit operator int(Index index) => index.ToInt32();

    public static bool TryFrom(int value, out Index index)
    {
      if(value < 0)
      {
        index = default;

        return false;
      }

      index = new Index(value);

      return true;
    }

    public static Index From(int value)
    {
      if(!TryFrom(value, out var index))
      {
        throw new ArgumentOutOfRangeException(nameof(value), value, "Value cannot be negative");
      }

      return index;
    }

    public sealed class Converter : TypeConverter
    {
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
        sourceType == typeof(int) || sourceType == typeof(string);

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
        destinationType == typeof(int) || destinationType == typeof(string);

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
      {
        switch(value)
        {
          case int i: return From(i);
          case string s: return From(int.Parse(s));
          default: return base.ConvertFrom(context, culture, value);
        }
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
      {
        var index = (Index) value;

        if(destinationType == typeof(int))
        {
          return index.ToInt32();
        }
        else if(destinationType == typeof(string))
        {
          return index.ToString();
        }
        else
        {
          return base.ConvertTo(context, culture, value, destinationType);
        }
      }
    }
  }
}