using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;

namespace Totem
{
	/// <summary>
	/// A portion of the text representation of a link
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class LinkText : LinkPart, IEquatable<LinkText>
	{
		public const string TemplateStart = "{";
		public const string TemplateEnd = "}";

		private readonly bool _isTemplate;

		public LinkText(string value)
		{
			Value = value ?? "";

			_isTemplate = value.StartsWith(TemplateStart) && value.EndsWith(TemplateEnd);
		}

		public string Value { get; private set; }
		public int Length => Value.Length;
		public bool IsNone => Value.Length == 0;
		public override bool IsTemplate => _isTemplate;

		public override Text ToText() => Value;

		public override bool Equals(object obj)
		{
			return Equals(obj as LinkText);
		}

		public bool Equals(LinkText other)
		{
			return Eq.Values(this, other).Check(x => x.Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public static readonly LinkText None = new LinkText("");

		public static bool operator ==(LinkText x, LinkText y) => Eq.Op(x, y);
		public static bool operator !=(LinkText x, LinkText y) => Eq.OpNot(x, y);

		public static bool operator ==(LinkText x, string y) => Eq.Op(x?.Value ?? "", y);
		public static bool operator !=(LinkText x, string y) => Eq.OpNot(x?.Value ?? "", y);

		public static implicit operator LinkText(char text) => new LinkText(text.ToString());
		public static implicit operator LinkText(string value) => new LinkText(value);
		public static implicit operator LinkText(Text text) => new LinkText(text);

		public sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value) => new LinkText(value);
		}
	}
}