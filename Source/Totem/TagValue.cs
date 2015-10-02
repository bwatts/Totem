using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Totem
{
	/// <summary>
	/// The value of a tag in a taggable object
	/// </summary>
	public sealed class TagValue : IWritable
	{
		public TagValue(Tag tag, object content)
		{
			Tag = tag;
			Content = content;
		}

		public TagValue(Tag tag)
		{
			Tag = tag;
			Content = tag.ResolveDefaultValue();
		}

		public Tag Tag { get; private set; }
		public object Content { get; private set; }
		public bool IsUnset { get { return Content == Tag.UnsetValue; } }

		public sealed override string ToString()
		{
			return ToText();
		}

		public Text ToText()
		{
			return Tag.ToText().Write(" = ").WriteIf(IsUnset, "<unset>", Text.Of(Content));
		}
	}
}