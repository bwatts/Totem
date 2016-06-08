using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;
using Totem.Reflection;

namespace Totem.Http
{
	/// <summary>
	/// A set of name/value pairs which select resources
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class HttpQuery : LinkPart, IEquatable<HttpQuery>, IEnumerable<HttpQueryPair>
	{
		public const char PairSeparator = '&';

		public static readonly HttpQuery Empty = new HttpQuery(new Dictionary<LinkText, HttpQueryPair>());

		private readonly Dictionary<LinkText, HttpQueryPair> _pairsByKey;

		private HttpQuery(Dictionary<LinkText, HttpQueryPair> pairsByKey)
		{
			_pairsByKey = pairsByKey;
		}

		public int Count => _pairsByKey.Count;
		public IEnumerable<LinkText> Keys => _pairsByKey.Keys;
		public IEnumerable<LinkText> Values => _pairsByKey.Values.Select(pair => pair.Value);
		public bool IsEmpty => Count == 0;
		public bool IsNotEmpty => Count > 0;
		public override bool IsTemplate => _pairsByKey.Values.Any(pair => pair.IsTemplate);

		public override Text ToText()
		{
			return _pairsByKey.ToTextSeparatedBy(PairSeparator, pair => pair.Value.ToText());
		}

		public IEnumerator<HttpQueryPair> GetEnumerator()
		{
			return _pairsByKey.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool ContainsKey(LinkText key)
		{
			return _pairsByKey.ContainsKey(key);
		}

		public LinkText Get(LinkText key, bool strict = true)
		{
			HttpQueryPair pair;

			Expect(_pairsByKey.TryGetValue(key, out pair) || !strict, "Unknown key: " + Text.Of(key));

			return pair == null ? null : pair.Value;
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

		public override bool Equals(object obj)
		{
			return Equals(obj as HttpQuery);
		}

		public bool Equals(HttpQuery other)
		{
			return !ReferenceEquals(other, null) && _pairsByKey.Values.ToHashSet().SetEquals(other._pairsByKey.Values);
		}

		public override int GetHashCode()
		{
			return HashCode.CombineItems(Values);
		}

		public static bool operator ==(HttpQuery x, HttpQuery y) => Eq.Op(x, y);
		public static bool operator !=(HttpQuery x, HttpQuery y) => Eq.OpNot(x, y);

		//
		// Factory
		//

		public static HttpQuery From(IEnumerable<HttpQueryPair> pairs)
		{
			return new HttpQuery(pairs.ToDictionary(pair => pair.Key));
		}

		public static HttpQuery From(params HttpQueryPair[] pairs)
		{
			return From(pairs as IEnumerable<HttpQueryPair>);
		}

		public static HttpQuery From(string value, bool strict = true)
		{
			var pairs = new List<HttpQueryPair>();

			foreach(var part in value.Split(PairSeparator))
			{
				var pair = HttpQueryPair.From(part, strict: false);

				if(pair == null)
				{
					ExpectNot(strict, "Failed to parse query: " + value);

					return null;
				}

				pairs.Add(pair);
			}

			return From(pairs);
		}

		public sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				return From(value);
			}
		}
	}
}