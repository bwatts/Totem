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
	[TypeConverter(typeof(HttpQuery.Converter))]
	public sealed class HttpQuery : LinkPart, IEquatable<HttpQuery>, IEnumerable<HttpQueryPair>
	{
		public const char PairSeparator = '&';

		public static readonly HttpQuery Empty = new HttpQuery(new Dictionary<LinkText, HttpQueryPair>());

		private readonly Dictionary<LinkText, HttpQueryPair> _pairsByKey;

		private HttpQuery(Dictionary<LinkText, HttpQueryPair> pairsByKey)
		{
			_pairsByKey = pairsByKey;
		}

		public int Count { get { return _pairsByKey.Count; } }
		public IEnumerable<LinkText> Keys { get { return _pairsByKey.Keys; } }
		public IEnumerable<LinkText> Values { get { return _pairsByKey.Values.Select(pair => pair.Value); } }
		public bool IsEmpty { get { return Count == 0; } }
		public bool IsNotEmpty { get { return Count > 0; } }
		public override bool IsTemplate { get { return _pairsByKey.Values.Any(pair => pair.IsTemplate); } }

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

		public LinkText GetValue(LinkText key, bool strict = true)
		{
			HttpQueryPair pair;

			Expect(_pairsByKey.TryGetValue(key, out pair) || !strict, "Unknown key: " + Text.Of(key));

			return pair == null ? null : pair.Value;
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

		public static bool operator ==(HttpQuery x, HttpQuery y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(HttpQuery x, HttpQuery y)
		{
			return !(x == y);
		}

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