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

		public void Write(FileResource file, Stream data, bool overwrite = true, bool createFolders = false)
		{
			var filePath = Link.Then(file).ToString();

			if(createFolders)
			{
				Write(file.Folder, overwrite);
			}

			using(var writeStream = File.OpenWrite(filePath))
			{
				data.CopyTo(writeStream);
			}
		}

		public void Write(FolderResource folder, bool overwrite = true)
		{
			var folderPath = Link.Then(folder).ToString();

			if(Directory.Exists(folderPath))
			{
				return;
			}

			Expect(overwrite, "Folder exists and overwrite option is not set");

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

		public Many<FileResource> ReadFiles(bool recursive = false)
		{
			return ReadFiles(FolderResource.Root, recursive);
		}

		public Many<FolderResource> ReadFolders(bool recursive = false)
		{
			return ReadFolders(FolderResource.Root, recursive);
		}

		public Many<IOLink> ReadLinks(bool recursive = false)
		{
			return ReadLinks(FolderResource.Root, recursive);
		}

		public Many<FileLink> ReadFileLinks(bool recursive = false)
		{
			return ReadFileLinks(FolderResource.Root, recursive);
		}

		public Many<FolderLink> ReadFolderLinks(bool recursive = false)
		{
			return ReadFolderLinks(FolderResource.Root, recursive);
		}

		//
		// Subfolder reads
		//

		public Many<IOResource> ReadFolder(FolderResource subfolder, bool recursive = false)
		{
			return ReadLinksCore(subfolder, recursive)
				.Select(link =>
				{
					return link is FolderLink
						? ((FolderLink) link).RelativeTo(Link)
						: ((FileLink) link).RelativeTo(Link) as IOResource;
				})
				.ToMany();
		}

		public Many<FileResource> ReadFiles(FolderResource subfolder, bool recursive = false)
		{
			return ReadFileLinksCore(subfolder, recursive).Select(fileLink => fileLink.RelativeTo(Link)).ToMany();
		}

		public Many<FolderResource> ReadFolders(FolderResource subfolder, bool recursive = false)
		{
			return ReadFolderLinksCore(subfolder, recursive).Select(folderLink => folderLink.RelativeTo(Link)).ToMany();
		}

		//
		// Read links
		//

		public Many<IOLink> ReadLinks(FolderResource subfolder, bool recursive = false)
		{
			return ReadLinksCore(subfolder, recursive).ToMany();
		}

		public Many<FileLink> ReadFileLinks(FolderResource subfolder, bool recursive = false)
		{
			return ReadFileLinksCore(subfolder, recursive).ToMany();
		}

		public Many<FolderLink> ReadFolderLinks(FolderResource subfolder, bool recursive = false)
		{
			return ReadFolderLinksCore(subfolder, recursive).ToMany();
		}

		private IEnumerable<IOLink> ReadLinksCore(FolderResource subfolder, bool recursive = false)
		{
			var folderPath = Link.Then(subfolder).ToString();
			var pattern = "*.*";
			var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			var files = Directory.GetFiles(folderPath, pattern, option).Select(file => FileLink.From(file));
			var directories = Directory.GetDirectories(folderPath, pattern, option).Select(subdirectory => FolderLink.From(subdirectory));

			return files.Concat<IOLink>(directories);
		}

		private IEnumerable<FileLink> ReadFileLinksCore(FolderResource subfolder, bool recursive = false)
		{
			var folderPath = Link.Then(subfolder).ToString();
			var pattern = "*.*";
			var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			return Directory.GetFiles(folderPath, pattern, option).Select(file => FileLink.From(file));
		}

		private IEnumerable<FolderLink> ReadFolderLinksCore(FolderResource subfolder, bool recursive = false)
		{
			var folderPath = Link.Then(subfolder).ToString();
			var pattern = "*.*";
			var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			return Directory.GetDirectories(folderPath, pattern, option).Select(subdirectory => FolderLink.From(subdirectory));
		}
	}
}