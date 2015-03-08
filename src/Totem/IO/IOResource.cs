using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// A resource targeted by an HTTP link
	/// </summary>
	[TypeConverter(typeof(IOResource.Converter))]
	public abstract class IOResource : LinkPart
	{
		internal IOResource()
		{}

		public sealed override Text ToText()
		{
			return ToText();
		}

		public abstract Text ToText(bool altSlash = false, bool leading = false);

		public static IOResource From(string value, bool strict = true)
		{
			var resource = FileResource.From(value, strict: false) ?? FolderResource.From(value, strict: false) as IOResource;

			Expect(strict && resource == null).IsFalse(
				issue: "Value is not an I/O resource",
				expected: "File or folder resource",
				actual: t => value);

			return resource;
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