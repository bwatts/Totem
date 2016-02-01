using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// A rooted reference to an IO resource
	/// </summary>
	[TypeConverter(typeof(IOLink.Converter))]
	public abstract class IOLink : LinkPart
	{
		internal IOLink()
		{}

		public sealed override Text ToText()
		{
			return ToText();
		}

		public abstract Text ToText(bool altSlash = false);

		public static IOLink From(string value, bool strict = true)
		{
			var link = FileLink.From(value, strict: false) ?? FolderLink.From(value, strict: false) as IOLink;

      ExpectNot(strict && link == null, "Value is not an I/O link");

			return link;
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