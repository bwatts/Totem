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

    public static bool TryFrom(Hex hex, out Sha1 sha1)
    {
      sha1 = hex.TextLength == HexLength ? new Sha1(hex) : null;

      return sha1 != null;
    }

    public static bool TryFrom(string hex, out Sha1 sha1)
    {
      sha1 = Hex.TryFrom(hex, out var parsedHex) && parsedHex.TextLength == HexLength ? new Sha1(parsedHex) : null;

      return sha1 != null;
    }

    public static bool TryFrom(Binary binary, out Sha1 sha1)
    {
      sha1 = binary.Length == BinaryLength ? new Sha1(Hex.From(binary)) : null;

      return sha1 != null;
    }

    public static Sha1 From(Hex hex)
    {
      if(!TryFrom(hex, out var sha1))
      {
        throw new FormatException($"Failed to parse hex, expected a SHA-1 text length of {HexLength}: {hex}");
      }

      return sha1;
    }

    public static Sha1 From(string hex)
    {
      if(!TryFrom(hex, out var sha1))
      {
        throw new FormatException($"Failed to parse hex, expected a SHA-1 text length of {HexLength}: {hex}");
      }

      return sha1;
    }

    public static Sha1 From(Binary binary)
    {
      if(!TryFrom(binary, out var sha1))
      {
        throw new FormatException($"Failed to parse binary, expected a SHA-1 byte length of {BinaryLength}: {binary}");
      }

      return sha1;
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