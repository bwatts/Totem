using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// A resource targeted by an I/O link
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public abstract class IOResource : LinkPart
	{
		internal IOResource()
		{}

		public sealed override Text ToText() => ToText();

		public abstract Text ToText(bool altSlash = false, bool leading = false);

		public static IOResource From(string value, bool strict = true)
		{
			var resource = FileResource.From(value, strict: false) ?? FolderResource.From(value, strict: false) as IOResource;

      ExpectNot(strict && resource == null, "Value is not an I/O resource");

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