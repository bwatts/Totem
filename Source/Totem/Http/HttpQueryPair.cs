using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;
using Totem.Reflection;

namespace Totem.Http
{
	/// <summary>
	/// A name/value pair that selects resources
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class HttpQueryPair : LinkPart, IEquatable<HttpQueryPair>
	{
		public const char Separator = '=';

		private HttpQueryPair(LinkText key, LinkText value)
		{
			Key = key;
			Value = value;
		}

		public LinkText Key { get; private set; }
		public LinkText Value { get; private set; }
		public override bool IsTemplate => Key.IsTemplate || Value.IsTemplate;

		public override Text ToText() => Key.ToText().Write(Separator).Write(Value);

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as HttpQueryPair);
		}

		public bool Equals(HttpQueryPair other)
		{
			return Eq.Values(this, other).Check(x => x.Key).Check(x => x.Value);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Key, Value);
		}

		public static bool operator ==(HttpQueryPair x, HttpQueryPair y) => Eq.Op(x, y);
		public static bool operator !=(HttpQueryPair x, HttpQueryPair y) => Eq.OpNot(x, y);

		//
		// Factory
		//

		public static HttpQueryPair From(LinkText key, LinkText value)
		{
			return new HttpQueryPair(key, value);
		}

		public static HttpQueryPair From(string value, bool strict = true)
		{
			var parts = value.Split(Separator);

			if(parts.Length != 2)
			{
				ExpectNot(strict, "Failed to parse query pair: " + value);

				return null;
			}

			return new HttpQueryPair(parts[0], parts[1]);
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