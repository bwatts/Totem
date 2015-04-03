using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

		void Write(FileResource file, Stream data, bool overwrite = true);

		void Write(FolderResource folder, bool overwrite = true);

		void Delete(FileResource file, bool strict = true);

		void Delete(FolderResource folder, bool strict = true);

		void Copy(FileResource sourceFile, FileResource destinationFile, bool overwrite = false);

		void Move(FileResource sourceFile, FileResource destinationFile);

		void Move(FolderResource sourceFolder, FolderResource destinationFolder);

		bool FileExists(FileResource file);

		bool FileExists(FileName file);

		bool FolderExists(FolderResource folder);

		Stream ReadFile(FileResource file, bool strict = true);

		IReadOnlyList<IOResource> ReadFolder(FolderResource folder, bool recursive = false);

		IReadOnlyList<FileResource> ReadFiles(FolderResource folder, bool recursive = false);

		IReadOnlyList<FolderResource> ReadFolders(FolderResource folder, bool recursive = false);

		IReadOnlyList<IOLink> ReadLinks(FolderResource folder, bool recursive = false);

		IReadOnlyList<FileLink> ReadFileLinks(FolderResource folder, bool recursive = false);

		IReadOnlyList<FolderLink> ReadFolderLinks(FolderResource folder, bool recursive = false);
	}
}