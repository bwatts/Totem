using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// A folder targeted by a link
	/// </summary>
	[TypeConverter(typeof(FolderResource.Converter))]
	public sealed class FolderResource : IOResource, IEquatable<FolderResource>
	{
		public static readonly FolderResource Root = new FolderResource(LinkPath.Root);

		private FolderResource(LinkPath path)
		{
			Path = path;
		}

		public LinkPath Path { get; private set; }
		public override bool IsTemplate { get { return Path.IsTemplate; } }

		public override Text ToText(bool altSlash = false, bool leading = false)
		{
			return ToText();
		}

		public Text ToText(bool altSlash = false, bool leading = false, bool trailing = false)
		{
			return Path.ToText(altSlash ? _separators[0] : _separators[1], leading, trailing);
		}

		public FolderResource RelativeTo(FolderResource other)
		{
			return new FolderResource(Path.RelativeTo(other.Path));
		}

		public FolderResource Up(int count = 1, bool strict = true)
		{
			return new FolderResource(Path.Up(count, strict));
		}

		public FolderResource Then(FolderResource folder)
		{
			return new FolderResource(Path.Then(folder.Path));
		}

		public FileResource Then(FileResource file)
		{
			return FileResource.From(Then(file.Folder), file.Name);
		}

		public FileResource Then(FileName file)
		{
			return FileResource.From(this, file);
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as FolderResource);
		}

		public bool Equals(FolderResource other)
		{
			return Equality.Check(this, other).Check(x => x.Path);
		}

		public override int GetHashCode()
		{
			return Path.GetHashCode();
		}

		public static bool operator ==(FolderResource x, FolderResource y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(FolderResource x, FolderResource y)
		{
			return !(x == y);
		}

		//
		// Factory
		//

		public static FolderResource From(LinkPath path)
		{
			return new FolderResource(path);
		}

		public new static FolderResource From(string text, bool strict = true)
		{
			return new FolderResource(LinkPath.From(text, _separators));
		}

		public static FolderResource FromRandom()
		{
			return From(System.IO.Path.GetRandomFileName());
		}

		private static readonly string[] _separators = new string[]
		{
			System.IO.Path.AltDirectorySeparatorChar.ToString(),
			System.IO.Path.DirectorySeparatorChar.ToString()
		};

		public new sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				return From(value);
			}
		}
	}
}