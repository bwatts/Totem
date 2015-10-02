using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// The extension portion of a file name
	/// </summary>
	public static class FileExtension
	{
		public const string Separator = ".";

		public static LinkText WithSeparator(LinkText extension)
		{
			return extension.Value.StartsWith(Separator) ? extension : new LinkText(Separator + extension.Value);
		}

		public static bool ExtensionIs(this FileName fileName, LinkText extension)
		{
			return fileName.Extension == extension;
		}

		public static bool ExtensionIsAny(this FileName fileName, IEnumerable<LinkText> extensions)
		{
			return extensions.Any(extension => fileName.ExtensionIs(extension));
		}

		public static bool ExtensionIsAny(this FileName fileName, params LinkText[] extensions)
		{
			return fileName.ExtensionIsAny(extensions as IEnumerable<LinkText>);
		}

		public static bool ExtensionIs(this FileResource file, LinkText extension)
		{
			return file.Name.ExtensionIs(extension);
		}

		public static bool ExtensionIsAny(this FileResource file, IEnumerable<LinkText> extensions)
		{
			return file.Name.ExtensionIsAny(extensions);
		}

		public static bool ExtensionIsAny(this FileResource file, params LinkText[] extensions)
		{
			return file.Name.ExtensionIsAny(extensions);
		}
	}
}