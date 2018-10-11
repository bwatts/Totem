using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
  /// <summary>
  /// A set of name/value pairs which select resources
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class HttpQuery : LinkPart, IEquatable<HttpQuery>, IEnumerable<HttpQueryPair>
  {
    public const char PairSeparator = '&';

    readonly Dictionary<LinkText, HttpQueryPair> _pairsByKey;

    HttpQuery(Dictionary<LinkText, HttpQueryPair> pairsByKey)
    {
      _pairsByKey = pairsByKey;
    }

    public int Count => _pairsByKey.Count;
    public LinkText this[LinkText key] => _pairsByKey[key].Value;
    public IEnumerable<LinkText> Keys => _pairsByKey.Keys;
    public IEnumerable<LinkText> Values => _pairsByKey.Values.Select(pair => pair.Value);
    public bool IsEmpty => Count == 0;
    public bool IsNotEmpty => Count > 0;
    public override bool IsTemplate => _pairsByKey.Values.Any(pair => pair.IsTemplate);

    public override string ToString() =>
      _pairsByKey.ToTextSeparatedBy(PairSeparator, pair => pair.Value.ToString());

    public IEnumerator<HttpQueryPair> GetEnumerator() =>
      _pairsByKey.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
      GetEnumerator();

    public bool ContainsKey(LinkText key) =>
      _pairsByKey.ContainsKey(key);

    public bool TryGet(LinkText key, out LinkText value)
    {
      value = _pairsByKey.TryGetValue(key, out var pair) ? pair.Value : null;

      return value != null;
    }

    public HttpQuery Set(LinkText key, LinkText value)
    {
      var newPairsByKey = _pairsByKey.ToDictionary();

      newPairsByKey[key] = HttpQueryPair.From(key, value);

      return new HttpQuery(newPairsByKey);
    }

    public HttpQuery Clear(LinkText key)
    {
      var newPairsByKey = _pairsByKey.ToDictionary();

      newPairsByKey.Remove(key);

      return new HttpQuery(newPairsByKey);
    }

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as HttpQuery);

    public bool Equals(HttpQuery other) =>
      !(other is null) && _pairsByKey.Values.ToHashSet().SetEquals(other._pairsByKey.Values);

    public override int GetHashCode() =>
      HashCode.CombineItems(Values);

    public static bool operator ==(HttpQuery x, HttpQuery y) => Eq.Op(x, y);
    public static bool operator !=(HttpQuery x, HttpQuery y) => Eq.OpNot(x, y);

    //
    // Factory
    //

    public static readonly HttpQuery Empty = new HttpQuery(new Dictionary<LinkText, HttpQueryPair>());

    public static bool TryFrom(string value, out HttpQuery query)
    {
      var pairs = new List<HttpQueryPair>();

      foreach(var part in value.Split(PairSeparator))
      {
        if(!HttpQueryPair.TryFrom(part, out var parsedPair))
        {
          query = null;

          return false;
        }

        pairs.Add(parsedPair);
      }

      query = From(pairs);

      return true;
    }

    public static HttpQuery From(IEnumerable<HttpQueryPair> pairs) =>
      new HttpQuery(pairs.ToDictionary(pair => pair.Key));

    public static HttpQuery From(params HttpQueryPair[] pairs) =>
      From(pairs as IEnumerable<HttpQueryPair>);

    public static HttpQuery From(string value) =>
      From(value.Split(PairSeparator).Select(HttpQueryPair.From));

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}