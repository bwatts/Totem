using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// A rooted reference to a folder resource
	/// </summary>
	[TypeConverter(typeof(FolderLink.Converter))]
	public sealed class FolderLink : IOLink, IEquatable<FolderLink>
	{
		private FolderLink(LinkText root, FolderResource resource, bool isUnc = false)
		{
			Root = root;
			Resource = resource;
			IsUnc = isUnc;
		}

		public LinkText Root { get; private set; }
		public FolderResource Resource { get; private set; }
		public bool IsUnc { get; private set; }
		public override bool IsTemplate { get { return Root.IsTemplate || Resource.IsTemplate; } }

		public override Text ToText(bool altSlash = false)
		{
			return ToText();
		}

		public Text ToText(bool altSlash = false, bool trailing = false)
		{
			return Root.ToText().Write(Resource.ToText(altSlash, leading: true, trailing: trailing));
		}

		public FolderResource RelativeTo(FolderLink other)
		{
			return Root != other.Root ? FolderResource.Root : Resource.RelativeTo(other.Resource);
		}

		public FolderLink Up(int count = 1, bool strict = true)
		{
			return new FolderLink(Root, Resource.Up(count, strict));
		}

		public FolderLink Then(FolderResource folder)
		{
			return new FolderLink(Root, Resource.Then(folder));
		}

		public FileLink Then(FileResource file)
		{
			return FileLink.From(this, file);
		}

		public FileLink Then(FileName file)
		{
			return FileLink.From(this, FileResource.From(file));
		}

		public FolderLink Then(Func<FolderResource, FolderResource> folderFromRoot)
		{
			return Then(folderFromRoot(FolderResource.Root));
		}

		public FileLink Then(Func<FolderResource, FileResource> fileFromRoot)
		{
			return Then(fileFromRoot(FolderResource.Root));
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as FolderLink);
		}

		public bool Equals(FolderLink other)
		{
			return Equality.Check(this, other).Check(x => x.Root).Check(x => x.Resource);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Root, Resource);
		}

		public static bool operator ==(FolderLink x, FolderLink y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(FolderLink x, FolderLink y)
		{
			return !(x == y);
		}

		//
		// Factory
		//

		public static FolderLink From(LinkText root, FolderResource resource)
		{
			return new FolderLink(root, resource);
		}

		public static FolderLink From(LinkText root, string resource, bool strict = true)
		{
			var parsedResource = FolderResource.From(resource, strict);

			return parsedResource == null ? null : From(root, resource);
		}

		public new static FolderLink From(string value, bool strict = true)
		{
			var folder = FromUnc(value, strict: false) ?? FromLocal(value, strict: false);

			Expect(strict && folder == null).IsFalse("Cannot parse folder link");

			return folder;
		}

		public static FolderLink FromUnc(string value, bool strict = true)
		{
			if(!String.IsNullOrEmpty(value) && value.StartsWith(@"\\"))
			{
				var parsedFolder = FolderResource.From(value.Substring(2), strict);

				if(parsedFolder != null)
				{
					var root = @"\\" + parsedFolder.Path.Segments[0].ToString();

					var path = parsedFolder.Path.Segments.Count == 1
						? LinkPath.Root
						: LinkPath.From(parsedFolder.Path.Segments.Skip(1));

					return new FolderLink(root, FolderResource.From(path), isUnc: true);
				}
			}

			Expect(strict).IsFalse("Cannot parse UNC link: " + value);

			return null;
		}

		public static FolderLink FromLocal(string value, bool strict = true)
		{
			var parsedFolder = FolderResource.From(value, strict);

			if(parsedFolder != null && parsedFolder.Path.Segments.Count > 0)
			{
				var path = parsedFolder.Path.Segments.Count == 1
					? LinkPath.Root
					: LinkPath.From(parsedFolder.Path.Segments.Skip(1));

				return new FolderLink(parsedFolder.Path.Segments[0], FolderResource.From(path));
			}

			Expect(strict).IsFalse("Cannot parse folder link: " + value);

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