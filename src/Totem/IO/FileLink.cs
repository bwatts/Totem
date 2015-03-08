using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// A rooted reference to a file resource
	/// </summary>
	[TypeConverter(typeof(FileLink.Converter))]
	public sealed class FileLink : IOLink, IEquatable<FileLink>
	{
		private FileLink(FolderLink folder, FileName name)
		{
			Folder = folder;
			Name = name;
		}

		public FolderLink Folder { get; private set; }
		public FileName Name { get; private set; }
		public override bool IsTemplate { get { return Folder.IsTemplate || Name.IsTemplate; } }

		public override Text ToText(bool altSlash = false)
		{
			return Folder.ToText(altSlash, trailing: true).Write(Name);
		}

		public FileResource RelativeTo(FolderLink folder)
		{
			return FileResource.From(Folder.RelativeTo(folder), Name);
		}

		public FileLink Up(int count = 1, bool strict = true)
		{
			return new FileLink(Folder.Up(count, strict), Name);
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as FileLink);
		}

		public bool Equals(FileLink other)
		{
			return Equality.Check(this, other).Check(x => x.Folder).Check(x => x.Name);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Folder, Name);
		}

		public static bool operator ==(FileLink x, FileLink y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(FileLink x, FileLink y)
		{
			return !(x == y);
		}

		//
		// Factory
		//

		public static FileLink From(FolderLink folder, FileResource file)
		{
			return new FileLink(folder.Then(file.Folder), file.Name);
		}

		public static FileLink From(FolderLink folder, string file, bool strict = true)
		{
			var parsedFile = FileResource.From(file, strict);

			return parsedFile == null ? null : From(folder, parsedFile);
		}

		public static FileLink From(string folder, FileResource file, bool strict = true)
		{
			var parsedFolder = FolderLink.From(folder, strict);

			return parsedFolder == null ? null : From(parsedFolder, file);
		}

		public static FileLink From(string folder, string file, bool strict = true)
		{
			var parsedFolder = FolderLink.From(folder, strict);

			return parsedFolder == null ? null : From(parsedFolder, file, strict);
		}

		public new static FileLink From(string value, bool strict = true)
		{
			var parsedFolder = FolderLink.From(value, strict);

			if(parsedFolder != null)
			{
				var fileSegment = parsedFolder.Resource.Path.Segments.LastOrDefault();

				if(fileSegment != null)
				{
					var parsedName = FileName.From(fileSegment.ToString(), strict);

					if(parsedName != null)
					{
						return new FileLink(parsedFolder.Up(strict: false), parsedName);
					}
				}
			}

			Expect(strict).IsFalse(issue: "Cannot parse file link", actual: t => value);

			return null;
		}

		public new sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				return From(value);
			}
		}
	}
}