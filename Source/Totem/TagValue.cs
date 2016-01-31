using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// The value of a tag in a taggable object
	/// </summary>
	public sealed class TagValue : IWritable
	{
		public TagValue(Tag tag)
		{
			Tag = tag;
			Content = tag.ResolveDefault();
			IsUnset = true;
		}

		public TagValue(Tag tag, object content)
		{
			Tag = tag;
			Content = content;
			IsUnset = false;
		}

		public Tag Tag { get; }
		public object Content { get; private set; }
		public bool IsUnset { get; private set; }

		public sealed override string ToString() => ToText();
		public Text ToText() => Text.Of(Content);

		public void Set(object content)
		{
			Content = content;

			IsUnset = content == Tag.UnsetValue;
		}
	}
}