using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Totem.IO
{
  /// <summary>
  /// A binary value encoded as Base64 text
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class Base64 : IEquatable<Base64>, IComparable<Base64>
  {
    public static readonly Base64 None = new Base64("", Binary.None);

    readonly string _text;
    readonly Binary _data;

    Base64(string text, Binary data)
    {
      _text = text;
      _data = data;
    }

    public int TextLength => _text.Length;
    public int DataLength => _data.Length;
    public long DataLongLength => _data.LongLength;

    public override string ToString() => _text;
    public Binary ToBinary() => _data;
    public byte[] ToBytes() => _data.ToBytes();

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as Base64);

    public bool Equals(Base64 other) =>
      Eq.Values(this, other).Check(x => x._text);

    public override int GetHashCode() =>
      _text.GetHashCode();

    public int CompareTo(Base64 other) =>
      Cmp.Values(this, other).Check(x => x._text);

    public static bool operator ==(Base64 x, Base64 y) => Eq.Op(x, y);
    public static bool operator !=(Base64 x, Base64 y) => Eq.OpNot(x, y);
    public static bool operator >(Base64 x, Base64 y) => Cmp.Op(x, y) > 0;
    public static bool operator <(Base64 x, Base64 y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(Base64 x, Base64 y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(Base64 x, Base64 y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    public static bool TryFrom(string value, out Base64 base64, Encoding encoding = null)
    {
      try
      {
        var data = Convert.FromBase64String(value);

        base64 = new Base64(value, Binary.From(data));

        return true;
      }
      catch(FormatException)
      {
        base64 = null;

        return false;
      }
    }

    public static Base64 From(string value, Encoding encoding = null)
    {
      var data = Convert.FromBase64String(value);

      return new Base64(value, Binary.From(data));
    }

    public static Base64 From(Binary value) =>
      new Base64(Convert.ToBase64String(value.ToBytes()), value);

    public sealed class Converter : BinaryConverter
    {
      protected override object ConvertFromText(ITypeDescriptorContext context, CultureInfo culture, Text value) =>
        From(value);

      protected override object ConvertFromBytes(ITypeDescriptorContext context, CultureInfo culture, IEnumerable<byte> value) =>
        From(Binary.From(value));

      protected override Text ConvertToText(ITypeDescriptorContext context, CultureInfo culture, object value) =>
        ((Base64) value).ToString();

      protected override IEnumerable<byte> ConvertToBytes(ITypeDescriptorContext context, CultureInfo culture, object value) =>
        ((Base64) value).ToBinary().ToBytes();
    }
  }
}