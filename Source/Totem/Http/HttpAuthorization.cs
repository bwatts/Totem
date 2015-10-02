using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;

namespace Totem.Http
{
	/// <summary>
	/// The value of the HTTP Authorization header
	/// </summary>
	[TypeConverter(typeof(HttpAuthorization.Converter))]
	public sealed class HttpAuthorization : Notion, IEquatable<HttpAuthorization>, IComparable<HttpAuthorization>
	{
		private HttpAuthorization(string type, string credentials)
		{
			Type = type;
			Credentials = credentials;
		}

		public string Type { get; private set; }
		public string Credentials { get; private set; }
		public bool IsAnonymous { get { return Type == "" || Credentials == ""; } }
		public bool IsAuthenticated { get { return Credentials != ""; } }

		public override Text ToText()
		{
			return Type.ToText().WriteIf(IsAuthenticated, Text.Of(" ").Write(Credentials));
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as HttpAuthorization);
		}

		public bool Equals(HttpAuthorization other)
		{
			return Equality.Check(this, other).Check(x => x.Type).Check(x => x.Credentials);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Type, Credentials);
		}

		public int CompareTo(HttpAuthorization other)
		{
			return Equality.Compare(this, other).Check(x => x.Type).Check(x => x.Credentials);
		}

		//
		// Operators
		//

		public static bool operator ==(HttpAuthorization x, HttpAuthorization y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(HttpAuthorization x, HttpAuthorization y)
		{
			return !(x == y);
		}

		public static bool operator >(HttpAuthorization x, HttpAuthorization y)
		{
			return Equality.CompareOp(x, y) > 0;
		}

		public static bool operator <(HttpAuthorization x, HttpAuthorization y)
		{
			return Equality.CompareOp(x, y) < 0;
		}

		public static bool operator >=(HttpAuthorization x, HttpAuthorization y)
		{
			return Equality.CompareOp(x, y) >= 0;
		}

		public static bool operator <=(HttpAuthorization x, HttpAuthorization y)
		{
			return Equality.CompareOp(x, y) <= 0;
		}

		//
		// Factory
		//

		public static readonly HttpAuthorization Anonymous = new HttpAuthorization("", "");

		public static HttpAuthorization From(string type, string credentials, bool strict = true)
		{
			if(type == "")
			{
				Expect(credentials != "" && strict).IsFalse("Credentials cannot be provided without a type");

				return Anonymous;
			}

			return new HttpAuthorization(type, credentials);
		}

		public static HttpAuthorization From(string value, bool strict = true)
		{
			if(value == "")
			{
				return Anonymous;
			}

			var parts = value.Split(' ');

			if(parts.Length != 2)
			{
				Expect(strict).IsFalse("Unable to parse value: " + value);

				return null;
			}

			return From(parts[0], parts[1], strict);
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