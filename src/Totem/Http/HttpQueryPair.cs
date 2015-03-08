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
	[TypeConverter(typeof(HttpQueryPair.Converter))]
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
		public override bool IsTemplate { get { return Key.IsTemplate || Value.IsTemplate; } }

		public override Text ToText()
		{
			return Key.ToText().Write(Separator).Write(Value);
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as HttpQueryPair);
		}

		public bool Equals(HttpQueryPair other)
		{
			return Equality.Check(this, other).Check(x => x.Key).Check(x => x.Value);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Key, Value);
		}

		public static bool operator ==(HttpQueryPair x, HttpQueryPair y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(HttpQueryPair x, HttpQueryPair y)
		{
			return !(x == y);
		}

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
				Expect(strict).IsFalse("Failed to parse query pair: " + value);

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