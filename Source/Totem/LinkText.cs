using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;
using Totem.Reflection;

namespace Totem
{
	/// <summary>
	/// A portion of the text representation of a link
	/// </summary>
	[TypeConverter(typeof(LinkText.Converter))]
	public sealed class LinkText : LinkPart, IEquatable<LinkText>
	{
		public const string TemplateStart = "{";
		public const string TemplateEnd = "}";

		private readonly bool _isTemplate;

		public LinkText(string value)
		{
			Value = value;

			_isTemplate = value.StartsWith(TemplateStart) && value.EndsWith(TemplateEnd);
		}

		public string Value { get; private set; }
		public int Length { get { return Value.Length; } }
		public bool IsNone { get { return Value.Length == 0; } }
		public override bool IsTemplate { get { return _isTemplate; } }

		public override Text ToText()
		{
			return Value;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as LinkText);
		}

		public bool Equals(LinkText other)
		{
			return Equality.Check(this, other).Check(x => x.Value);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public static readonly LinkText None = new LinkText("");

		public static bool operator ==(LinkText x, LinkText y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(LinkText x, LinkText y)
		{
			return !(x == y);
		}

		public static bool operator ==(LinkText x, string y)
		{
			return Equality.CheckOp(x.Value, y);
		}

		public static bool operator !=(LinkText x, string y)
		{
			return !(x == y);
		}

		public static implicit operator LinkText(char text)
		{
			return new LinkText(text.ToString());
		}

		public static implicit operator LinkText(string value)
		{
			return new LinkText(value);
		}

		public static implicit operator LinkText(Text text)
		{
			return new LinkText(text);
		}

		public sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				return new LinkText(value);
			}
		}
	}
}