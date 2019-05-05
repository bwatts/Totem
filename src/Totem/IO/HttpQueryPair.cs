using System;
using System.ComponentModel;

namespace Totem.IO
{
  /// <summary>
  /// A name/value pair that selects resources
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class HttpQueryPair : LinkPart, IEquatable<HttpQueryPair>
  {
    public const char Separator = '=';

    HttpQueryPair(LinkText key, LinkText value)
    {
      Key = key;
      Value = value;
    }

    public readonly LinkText Key;
    public readonly LinkText Value;

    public override bool IsTemplate =>
      Key.IsTemplate || Value.IsTemplate;

    public override string ToString() =>
      Text.Of(Key).Write(Separator).Write(Value);

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as HttpQueryPair);

    public bool Equals(HttpQueryPair other) =>
      Eq.Values(this, other).Check(x => x.Key).Check(x => x.Value);

    public override int GetHashCode() =>
      HashCode.Combine(Key, Value);

    public static bool operator ==(HttpQueryPair x, HttpQueryPair y) => Eq.Op(x, y);
    public static bool operator !=(HttpQueryPair x, HttpQueryPair y) => Eq.OpNot(x, y);

    //
    // Factory
    //

    public static bool TryFrom(string value, out HttpQueryPair pair)
    {
      var parts = value.Split(Separator);

      pair = parts.Length == 2 ? new HttpQueryPair(parts[0], parts[1]) : null;

      return pair != null;
    }

    public static HttpQueryPair From(LinkText key, LinkText value) =>
      new HttpQueryPair(key, value);

    public static HttpQueryPair From(string value)
    {
      if(!TryFrom(value, out var pair))
      {
        throw new FormatException($"Failed to parse query pair: {value}");
      }

      return pair;
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}