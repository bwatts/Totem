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
	[TypeConverter(typeof(Converter))]
	public sealed class MediaType : Notion, IEquatable<MediaType>, IComparable<MediaType>
	{
		public MediaType(string name)
		{
			Name = name;
		}

		public readonly string Name;

		public override Text ToText() => ToText();

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

		public static bool operator ==(MediaType x, MediaType y) => Eq.Op(x, y);
		public static bool operator !=(MediaType x, MediaType y) => Eq.OpNot(x, y);
		public static bool operator >(MediaType x, MediaType y) => Cmp.Op(x, y) > 0;
		public static bool operator <(MediaType x, MediaType y) => Cmp.Op(x, y) < 0;
		public static bool operator >=(MediaType x, MediaType y) => Cmp.Op(x, y) >= 0;
		public static bool operator <=(MediaType x, MediaType y) => Cmp.Op(x, y) <= 0;

		public static implicit operator MediaType(string name) => new MediaType(name);

		//
		// Inherent
		//

		public static class Names
		{
			public const string Css = "text/css";
			public const string Html = "text/html";
			public const string Javascript = "application/javascript";
			public const string Json = "application/json";
			public const string Plain = "text/plain";
		}

		public static readonly MediaType Css = new MediaType(Names.Css);
		public static readonly MediaType Html = new MediaType(Names.Html);
		public static readonly MediaType Javascript = new MediaType(Names.Javascript);
		public static readonly MediaType Json = new MediaType(Names.Json);
		public static readonly MediaType Plain = new MediaType(Names.Plain);

		public static readonly Many<MediaType> AllKnown = Many.Of(Css, Html, Javascript, Json, Plain);
		public static readonly Many<MediaType> AllText = Many.Of(Css, Html, Javascript, Json, Plain);

		public sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				return new MediaType(value);
			}
		}
	}
}