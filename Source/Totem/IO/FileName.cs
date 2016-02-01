using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// The extension-qualified name identifying a file
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class FileName : LinkPart, IEquatable<FileName>
	{
		private FileName(LinkText text, LinkText extension)
		{
			Text = text;
			Extension = extension;
		}

		public LinkText Text { get; private set; }
		public LinkText Extension { get; private set; }
		public override bool IsTemplate => Text.IsTemplate || Extension.IsTemplate;

		public override Text ToText()
		{
			return Text.ToText().WriteIf(Extension.Length > 0, FileExtension.WithSeparator(Extension).ToText());
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as FileName);
		}

		public bool Equals(FileName other)
		{
			return Eq.Values(this, other).Check(x => x.Text).Check(x => x.Extension);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Text, Extension);
		}

		public static bool operator ==(FileName x, FileName y) => Eq.Op(x, y);
		public static bool operator !=(FileName x, FileName y) => Eq.OpNot(x, y);

		//
		// Factory
		//

		public static FileName From(string value, bool strict = true, bool extensionOptional = false)
		{
			var extensionIndex = value.LastIndexOf(FileExtension.Separator);

			if(extensionIndex == -1)
			{
				if(extensionOptional)
				{
					return new FileName(value, "");
				}

				ExpectNot(strict, "Extension is required");

				return null;
			}
			
			if(extensionIndex == value.Length - 1)
			{
				ExpectNot(strict, "A file name cannot end with an extension separator");

				return null;
			}

			return new FileName(
				value.Substring(0, extensionIndex),
				value.Substring(extensionIndex + 1));
		}

		public static FileName From(LinkText text, LinkText extension)
		{
			return new FileName(text, extension);
		}

		public static FileName FromRandom()
		{
			return From(System.IO.Path.GetRandomFileName());
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