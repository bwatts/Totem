﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// A rooted reference to an IO resource
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public abstract class IOLink : LinkPart
	{
		internal IOLink()
		{}

		public sealed override Text ToText() => ToText();

		public abstract Text ToText(bool altSlash = false);

		public static IOLink From(string value, bool strict = true, bool extensionOptional = false)
		{
			var link = FileLink.From(value, strict: false, extensionOptional: extensionOptional) ?? FolderLink.From(value, strict: false) as IOLink;

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