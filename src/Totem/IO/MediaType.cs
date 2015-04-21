using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Totem.IO
{
	/// <summary>
	/// An identifier representing the format of a piece of media
	/// </summary>
	[TypeConverter(typeof(MediaType.Converter))]
	public sealed class MediaType : Notion, IEquatable<MediaType>, IComparable<MediaType>
	{
		public MediaType(string name)
		{
			Name = name;
		}

		public readonly string Name;

		public override Text ToText()
		{
			return ToText();
		}

		public Text ToText(bool escaped = false, Encoding encoding = null)
		{
			var text = escaped ? Uri.EscapeDataString(Name) : Name;

			if(encoding != null)
			{
				text += "; charset=" + encoding.WebName;
			}

			return text;
		}

		public Text ToTextUtf8(bool escaped = false)
		{
			return ToText(escaped, Encoding.UTF8);
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as MediaType);
		}

		public bool Equals(MediaType other)
		{
			return !ReferenceEquals(other, null) && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public int CompareTo(MediaType other)
		{
			return other == null ? 1 : String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
		}

		//
		// Operators
		//

		public static implicit operator MediaType(string name)
		{
			return new MediaType(name);
		}

		public static bool operator ==(MediaType x, MediaType y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(MediaType x, MediaType y)
		{
			return !(x == y);
		}

		public static bool operator >(MediaType x, MediaType y)
		{
			return Equality.CompareOp(x, y) > 0;
		}

		public static bool operator <(MediaType x, MediaType y)
		{
			return Equality.CompareOp(x, y) < 0;
		}

		public static bool operator >=(MediaType x, MediaType y)
		{
			return Equality.CompareOp(x, y) >= 0;
		}

		public static bool operator <=(MediaType x, MediaType y)
		{
			return Equality.CompareOp(x, y) <= 0;
		}

		//
		// Inherent
		//

		public static readonly MediaType Css = new MediaType("text/css");
		public static readonly MediaType Html = new MediaType("text/html");
		public static readonly MediaType Javascript = new MediaType("application/javascript");
		public static readonly MediaType Json = new MediaType("application/json");
		public static readonly MediaType Plain = new MediaType("text/plain");

		public static readonly IReadOnlyList<MediaType> AllKnown = Many.Of(Css, Html, Javascript, Json, Plain).AsReadOnly();
		public static readonly IReadOnlyList<MediaType> AllText = Many.Of(Css, Html, Javascript, Json, Plain).AsReadOnly();

		public sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				return new MediaType(value);
			}
		}
	}
}