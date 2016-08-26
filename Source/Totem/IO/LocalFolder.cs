using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

		public override Text ToText() => Link.ToText();

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

		public void Write(bool overwrite = true)
		{
			Write(FolderResource.Root, overwrite);
		}

		public Stream Write(FileResource file, bool overwrite = true, bool createFolders = false)
		{
			var filePath = Link.Then(file).ToString();

			if(createFolders)
			{
				Directory.CreateDirectory(Link.Then(file.Folder).ToString());
			}

			var mode = overwrite ? FileMode.Create : FileMode.Append;

			return File.Open(filePath, mode, FileAccess.Write);
		}

		public void Write(FileResource file, Stream data, bool overwrite = true, bool createFolders = false)
		{
			using(var openFile = Write(file, overwrite, createFolders))
			{
				data.CopyTo(openFile);
			}
		}

		public void Write(FolderResource folder, bool overwrite = true)
		{
			var folderPath = Link.Then(folder).ToString();

			if(overwrite && Directory.Exists(folderPath))
			{
				Directory.Delete(folderPath, recursive: true);
			}

			Directory.CreateDirectory(folderPath);
		}

		//
		// Delete
		//

		public void Delete(bool strict = true)
		{
			Delete(FolderResource.Root, strict);
		}

		public void Delete(FileName file, bool strict = true)
		{
			Delete(FileResource.From(file), strict);
		}

		public void Delete(FileResource file, bool strict = true)
		{
			var filePath = Link.Then(file).ToString();

			if(File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			else
			{
				ExpectNot(strict, "File not found");
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
					ExpectNot(strict, "Folder not found");
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

		public bool FolderExists(FolderResource subfolder)
		{
			return Directory.Exists(Link.Then(subfolder).ToString());
		}

		//
		// Reads
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

		public Many<FileResource> ReadFiles(bool recursive = false, string pattern = "*.*")
		{
			return ReadFiles(FolderResource.Root, recursive, pattern);
		}

		public Many<FolderResource> ReadFolders(bool recursive = false, string pattern = "*.*")
		{
			return ReadFolders(FolderResource.Root, recursive, pattern);
		}

		public Many<IOLink> ReadLinks(bool recursive = false, string pattern = "*.*")
		{
			return ReadLinks(FolderResource.Root, recursive, pattern);
		}

		public Many<FileLink> ReadFileLinks(bool recursive = false, string pattern = "*.*")
		{
			return ReadFileLinks(FolderResource.Root, recursive, pattern);
		}

		public Many<FolderLink> ReadFolderLinks(bool recursive = false, string pattern = "*.*")
		{
			return ReadFolderLinks(FolderResource.Root, recursive, pattern);
		}

		//
		// Subfolder reads
		//

		public Many<IOResource> ReadFolder(FolderResource subfolder, bool recursive = false, string pattern = "*.*")
		{
			return ReadLinksCore(subfolder, recursive, pattern).ToMany(link =>
				link is FolderLink
					? ((FolderLink) link).RelativeTo(Link)
					: ((FileLink) link).RelativeTo(Link) as IOResource);
		}

		public Many<FileResource> ReadFiles(FolderResource subfolder, bool recursive = false, string pattern = "*.*")
		{
			return ReadFileLinksCore(subfolder, recursive, pattern).ToMany(fileLink => fileLink.RelativeTo(Link));
		}

		public Many<FolderResource> ReadFolders(FolderResource subfolder, bool recursive = false, string pattern = "*.*")
		{
			return ReadFolderLinksCore(subfolder, recursive, pattern).ToMany(folderLink => folderLink.RelativeTo(Link));
		}

		//
		// Read links
		//

		public Many<IOLink> ReadLinks(FolderResource subfolder, bool recursive = false, string pattern = "*.*")
		{
			return ReadLinksCore(subfolder, recursive, pattern).ToMany();
		}

		public Many<FileLink> ReadFileLinks(FolderResource subfolder, bool recursive = false, string pattern = "*.*")
		{
			return ReadFileLinksCore(subfolder, recursive, pattern).ToMany();
		}

		public Many<FolderLink> ReadFolderLinks(FolderResource subfolder, bool recursive = false, string pattern = "*.*")
		{
			return ReadFolderLinksCore(subfolder, recursive, pattern).ToMany();
		}

		private IEnumerable<IOLink> ReadLinksCore(FolderResource subfolder, bool recursive, string pattern)
		{
			var folderPath = Link.Then(subfolder).ToString();
			var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			var files = Directory.GetFiles(folderPath, pattern, option).Select(file => FileLink.From(file, extensionOptional: true));
			var directories = Directory.GetDirectories(folderPath, pattern, option).Select(subdirectory => FolderLink.From(subdirectory));

			return files.Concat<IOLink>(directories);
		}

		private IEnumerable<FileLink> ReadFileLinksCore(FolderResource subfolder, bool recursive, string pattern)
		{
			var folderPath = Link.Then(subfolder).ToString();
			var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			return Directory.GetFiles(folderPath, pattern, option).Select(file => FileLink.From(file, extensionOptional: true));
		}

		private IEnumerable<FolderLink> ReadFolderLinksCore(FolderResource subfolder, bool recursive, string pattern)
		{
			var folderPath = Link.Then(subfolder).ToString();
			var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			return Directory.GetDirectories(folderPath, pattern, option).Select(subdirectory => FolderLink.From(subdirectory));
		}
	}
}