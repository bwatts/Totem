using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// Describes a folder in a file system
	/// </summary>
	public interface IFolder : IWritable
	{
		FolderLink Link { get; }

		IFolder Up(int count = 1, bool strict = true);

		IFolder Then(FolderResource folder);

		void Write(bool overwrite = true);

		Stream Write(FileResource file, bool overwrite = true, bool createFolders = false);

		void Write(FileResource file, Stream data, bool overwrite = true, bool createFolders = false);

		void Write(FolderResource folder, bool overwrite = true);

		void Delete(bool strict = true);

		void Delete(FileName file, bool strict = true);

		void Delete(FileResource file, bool strict = true);

		void Delete(FolderResource folder, bool strict = true);

		void Copy(FileResource sourceFile, FileResource destinationFile, bool overwrite = false);

		void Move(FileResource sourceFile, FileResource destinationFile);

		void Move(FolderResource sourceFolder, FolderResource destinationFolder);

		bool FileExists(FileResource file);

		bool FileExists(FileName file);

		bool FolderExists(FolderResource subfolder);

		Stream ReadFile(FileResource file, bool strict = true);

		Many<FileResource> ReadFiles(bool recursive = false, string pattern = "*.*");

		Many<FolderResource> ReadFolders(bool recursive = false, string pattern = "*.*");

		Many<IOLink> ReadLinks(bool recursive = false, string pattern = "*.*");

		Many<FileLink> ReadFileLinks(bool recursive = false, string pattern = "*.*");

		Many<FolderLink> ReadFolderLinks(bool recursive = false, string pattern = "*.*");

		Many<IOResource> ReadFolder(FolderResource subfolder, bool recursive = false, string pattern = "*.*");

		Many<FileResource> ReadFiles(FolderResource subfolder, bool recursive = false, string pattern = "*.*");

		Many<FolderResource> ReadFolders(FolderResource subfolder, bool recursive = false, string pattern = "*.*");

		Many<IOLink> ReadLinks(FolderResource subfolder, bool recursive = false, string pattern = "*.*");

		Many<FileLink> ReadFileLinks(FolderResource subfolder, bool recursive = false, string pattern = "*.*");

		Many<FolderLink> ReadFolderLinks(FolderResource subfolder, bool recursive = false, string pattern = "*.*");
	}
}