using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Totem.IO
{
  /// <summary>
  /// A sequences of bytes
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public struct Binary : IEquatable<Binary>
  {
    readonly byte[] _data;

    Binary(byte[] data) : this()
    {
      _data = data;
    }

    public int Length => _data == null ? 0 : _data.Length;
    public long LongLength => _data == null ? 0 : _data.LongLength;

    public override string ToString() =>
      _data == null ? "" : ToBase64().ToString();

    public string ToString(Encoding encoding) =>
      encoding.GetString(_data);

    public string ToStringUtf8() =>
      ToString(Encoding.UTF8);

    public byte[] ToBytes()
    {
      var copy = new byte[_data.Length];

      _data.CopyTo(copy, 0);

      return copy;
    }

    public Base64 ToBase64() =>
      Base64.From(this);

    public Hex ToHex() =>
      Hex.From(this);

    public MemoryStream ToStream() =>
      new MemoryStream(_data);

    public Binary Append(Binary other)
    {
      var data = new byte[Length + other.Length];

      _data.CopyTo(data, 0);

      other._data.CopyTo(data, other.Length);

      return new Binary(data);
    }

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      obj is Binary && Equals((Binary) obj);

    public bool Equals(Binary other) =>
      other != null && (_data == other._data || (_data != null && _data.SequenceEqual(other._data)));

    public override int GetHashCode() =>
      _data == null ? 0 : HashCode.CombineItems(_data);

    //
    // Operators
    //

    public static bool operator ==(Binary x, Binary y) => Eq.Op(x, y);
    public static bool operator !=(Binary x, Binary y) => Eq.OpNot(x, y);

    public static implicit operator Binary(byte[] data) =>
      From(data);

    public static implicit operator byte[](Binary value) =>
      value.ToBytes();

    public static Binary operator +(Binary x, Binary y) =>
      x.Append(y);

    //
    // Factory
    //

    public static readonly Binary None = new Binary();

    public static Binary From(byte[] value) =>
      new Binary(value);

    public static Binary From(IEnumerable<byte> value) =>
      new Binary(value.ToArray());

    public static Binary From(Stream value)
    {
      var data = new MemoryStream();

      value.CopyTo(data);

      return new Binary(data.ToArray());
    }

    public static Binary From(string value, Encoding encoding = null)
    {
      encoding = encoding ?? Encoding.Default;

      return new Binary(encoding.GetBytes(value));
    }

    public static Binary FromUtf8(string value) =>
      From(value, Encoding.UTF8);

    public static Binary Random(int byteCount)
    {
      var data = new byte[byteCount];

      new RNGCryptoServiceProvider().GetBytes(data);

      return new Binary(data);
    }

    public sealed class Converter : TypeConverter
    {
      //
      // From
      //

      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
        sourceType == typeof(string)
        || sourceType == typeof(Text)
        || typeof(IEnumerable<byte>).IsAssignableFrom(sourceType)
        || base.CanConvertFrom(context, sourceType);

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
      {
        if(value is string)
        {
          return From((string) value);
        }
        else if(value is Text)
        {
          return From((Text) value);
        }
        else if(value is IEnumerable<byte>)
        {
          return From((IEnumerable<byte>) value);
        }
        else
        {
          return base.ConvertFrom(context, culture, value);
        }
      }

      //
      // To
      //

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
        destinationType == typeof(string)
        || destinationType == typeof(Text)
        || destinationType == typeof(byte[])
        || destinationType == typeof(IEnumerable<byte>)
        || destinationType == typeof(IList<byte>)
        || destinationType == typeof(List<byte>)
        || base.CanConvertTo(context, destinationType);

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
      {
        if(value is Binary binary)
        {
          if(destinationType == typeof(string))
          {
            return binary.ToString();
          }
          else if(destinationType == typeof(Text))
          {
            return Text.Of(binary);
          }
          else if(destinationType == typeof(byte[]) || destinationType == typeof(IEnumerable<byte>))
          {
            return binary.ToBytes();
          }
          else
          {
            if(destinationType == typeof(IList<byte>) || destinationType == typeof(List<byte>))
            {
              return binary.ToBytes().ToList();
            }
          }
        }

        return base.ConvertTo(context, culture, value, destinationType);
      }
    }
  }
}