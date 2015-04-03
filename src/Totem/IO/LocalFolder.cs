using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.IO
{
	/// <summary>
	/// A folder on the local file system
	/// </summary>
	public sealed class LocalFolder : Notion, IFolder
	{
		public LocalFolder(FolderLink link)
		{
			Link = link;
		}

		public FolderLink Link { get; private set; }

		public override Text ToText()
		{
			return Link.ToText();
		}

		public IFolder Then(FolderResource folder)
		{
			return new LocalFolder(Link.Then(folder));
		}

		public IFolder Up(int count = 1, bool strict = true)
		{
			return new LocalFolder(Link.Up(count, strict));
		}

		//
		// Write
		//

		public void Write(FileResource file, Stream data, bool overwrite = true)
		{
			Write(file.Folder, overwrite);

			var filePath = Link.Then(file).ToString();

			using(var writeStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				data.CopyTo(writeStream);
			}
		}

		public void Write(FolderResource folder, bool overwrite = true)
		{
			var folderPath = Link.Then(folder).ToString();

			Expect(overwrite || !Directory.Exists(folderPath)).IsTrue(
				issue: "Folder exists and overwrite option is not set",
				actual: t => folder.ToText());

			Directory.CreateDirectory(folderPath);
		}

		//
		// Delete
		//

		public void Delete(FileResource file, bool strict = true)
		{
			var filePath = Link.Then(file).ToString();

			if(File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			else
			{
				Expect(strict).IsFalse(issue: "File not found", actual: t => filePath);
			}
		}

		public void Delete(FolderResource folder, bool strict = true)
		{
			var folderPath = Link.Then(folder).ToString();

			if(Directory.Exists(folderPath))
			{
				Directory.Delete(folderPath, recursive: true);
			}
			else
			{
				if(strict)
				{
					Expect(strict).IsFalse(issue: "Folder not found", actual: t => folderPath);
				}
			}
		}

		//
		// Copy
		//

		public void Copy(FileResource sourceFile, FileResource destinationFile, bool overwrite = false)
		{
			File.Copy(
				Link.Then(sourceFile).ToString(),
				Link.Then(destinationFile).ToString(),
				overwrite);
		}

		//
		// Move
		//

		public void Move(FileResource sourceFile, FileResource destinationFile)
		{
			File.Move(Link.Then(sourceFile).ToString(), Link.Then(destinationFile).ToString());
		}

		public void Move(FolderResource sourceFolder, FolderResource destinationFolder)
		{
			Directory.Move(Link.Then(sourceFolder).ToString(), Link.Then(destinationFolder).ToString());
		}

		//
		// Existence
		//

		public bool FileExists(FileResource file)
		{
			return File.Exists(Link.Then(file).ToString());
		}

		public bool FileExists(FileName file)
		{
			return File.Exists(Link.Then(file).ToString());
		}

		public bool FolderExists(FolderResource folder)
		{
			return Directory.Exists(Link.Then(folder).ToString());
		}

		//
		// Read files and folders
		//

		public Stream ReadFile(FileResource file, bool strict = true)
		{
			Stream data;

			try
			{
				data = new FileStream(Link.Then(file).ToString(), FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch(FileNotFoundException)
			{
				if(strict)
				{
					throw;
				}

				data = null;
			}

			return data;
		}

		public IReadOnlyList<IOResource> ReadFolder(FolderResource folder, bool recursive = false)
		{
			return ReadLinksCore(folder, recursive)
				.Select(link =>
				{
					return link is FolderLink
						? ((FolderLink) link).RelativeTo(Link)
						: ((FileLink) link).RelativeTo(Link) as IOResource;
				})
				.ToList();
		}

		public IReadOnlyList<FileResource> ReadFiles(FolderResource folder, bool recursive = false)
		{
			return ReadFileLinksCore(folder, recursive).Select(fileLink => fileLink.RelativeTo(Link)).ToList();
		}

		public IReadOnlyList<FolderResource> ReadFolders(FolderResource folder, bool recursive = false)
		{
			return ReadFolderLinksCore(folder, recursive).Select(folderLink => folderLink.RelativeTo(Link)).ToList();
		}

		//
		// Read links
		//

		public IReadOnlyList<IOLink> ReadLinks(FolderResource folder, bool recursive = false)
		{
			return ReadLinksCore(folder, recursive).ToList();
		}

		public IReadOnlyList<FileLink> ReadFileLinks(FolderResource folder, bool recursive = false)
		{
			return ReadFileLinksCore(folder, recursive).ToList();
		}

		public IReadOnlyList<FolderLink> ReadFolderLinks(FolderResource folder, bool recursive = false)
		{
			return ReadFolderLinksCore(folder, recursive).ToList();
		}

		private IEnumerable<IOLink> ReadLinksCore(FolderResource folder, bool recursive = false)
		{
			var folderPath = Link.Then(folder).ToString();
			var pattern = "*.*";
			var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			var files = Directory.GetFiles(folderPath, pattern, option).Select(file => FileLink.From(file));
			var directories = Directory.GetDirectories(folderPath, pattern, option).Select(subdirectory => FolderLink.From(subdirectory));

			return files.Concat<IOLink>(directories);
		}

		private IEnumerable<FileLink> ReadFileLinksCore(FolderResource folder, bool recursive = false)
		{
			var folderPath = Link.Then(folder).ToString();
			var pattern = "*.*";
			var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			return Directory.GetFiles(folderPath, pattern, option).Select(file => FileLink.From(file));
		}

		private IEnumerable<FolderLink> ReadFolderLinksCore(FolderResource folder, bool recursive = false)
		{
			var folderPath = Link.Then(folder).ToString();
			var pattern = "*.*";
			var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			return Directory.GetDirectories(folderPath, pattern, option).Select(subdirectory => FolderLink.From(subdirectory));
		}
	}
}