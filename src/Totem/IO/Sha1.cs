using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Totem.IO
{
  /// <summary>
  /// A SHA-1 value encoded as hexadecimal text
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class Sha1 : IEquatable<Sha1>, IComparable<Sha1>
  {
    public const int HexLength = 40;
    public const int BinaryLength = 20;

    readonly Hex _hex;

    Sha1(Hex hex)
    {
      _hex = hex;
    }

    public override string ToString() =>
      _hex.ToString();

    public Hex ToHex() =>
      _hex;

    public Binary ToBinary() =>
      _hex.ToBinary();

    public byte[] ToBytes() =>
      _hex.ToBytes();

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as Sha1);

    public bool Equals(Sha1 other) =>
      Eq.Values(this, other).Check(x => x._hex);

    public override int GetHashCode() =>
      _hex.GetHashCode();

    public int CompareTo(Sha1 other) =>
      Cmp.Values(this, other).Check(x => x._hex);

    public static bool operator ==(Sha1 x, Sha1 y) => Eq.Op(x, y);
    public static bool operator !=(Sha1 x, Sha1 y) => Eq.OpNot(x, y);
    public static bool operator >(Sha1 x, Sha1 y) => Cmp.Op(x, y) > 0;
    public static bool operator <(Sha1 x, Sha1 y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(Sha1 x, Sha1 y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(Sha1 x, Sha1 y) => Cmp.Op(x, y) <= 0;

    //
    // From
    //

    public static readonly Sha1 None = new Sha1(Hex.None);

    public static Sha1 From(Hex hex, bool strict = true)
    {
      if(hex.TextLength != HexLength)
      {
        Expect.False(strict, $"Value is not the SHA-1 text length of 40 characters: {hex}");

        return None;
      }

      return new Sha1(hex);
    }

    public static Sha1 From(string hex, bool strict = true) =>
      new Sha1(Hex.From(hex, strict));

    public static Sha1 From(Binary binary, bool strict = true)
    {
      if(binary.Length != BinaryLength)
      {
        Expect.False(strict, $"Value is not the SHA-1 binary length of 20 bytes: {binary}");

        return None;
      }

      return new Sha1(Hex.From(binary));
    }

    //
    // Compute
    //

    public static Sha1 Compute(Binary value)
    {
      var hash = new SHA1CryptoServiceProvider().ComputeHash(value.ToBytes());

      return From(Hex.From(Binary.From(hash)));
    }

    public static Sha1 Compute(Hex value) =>
      Compute(value.ToBinary());

    public static Sha1 Compute(string value, Encoding encoding = null) =>
      Compute(Binary.From(value, encoding));

    public sealed class Converter : BinaryConverter
    {
      protected override object ConvertFromText(ITypeDescriptorContext context, CultureInfo culture, Text value) =>
        From(value);

      protected override object ConvertFromBytes(ITypeDescriptorContext context, CultureInfo culture, IEnumerable<byte> value) =>
        From(Binary.From(value));

      protected override Text ConvertToText(ITypeDescriptorContext context, CultureInfo culture, object value) =>
        Text.Of(value);

      protected override IEnumerable<byte> ConvertToBytes(ITypeDescriptorContext context, CultureInfo culture, object value) =>
        ((Sha1) value).ToBytes();
    }
  }
}