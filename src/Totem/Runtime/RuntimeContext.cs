using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime
{
	/// <summary>
	/// The context of a set of artifacts defining the Totem runtime
	/// </summary>
	public class RuntimeContext
	{
		public RuntimeContext(
			bool inSolution,
			bool userInteractive,
			IFolder folder,
			IFolder dataFolder,
			LogLevel logLevel,
			IFolder logFolder,
			IReadOnlyList<string> packageNames)
		{
			InSolution = inSolution;
			UserInteractive = userInteractive;
			Folder = folder;
			DataFolder = dataFolder;
			LogLevel = logLevel;
			LogFolder = logFolder;
			PackageNames = packageNames;
		}

		public readonly bool InSolution;
		public readonly bool UserInteractive;
		public readonly IFolder Folder;
		public readonly IFolder DataFolder;
		public readonly LogLevel LogLevel;
		public readonly IFolder LogFolder;
		public readonly IReadOnlyList<string> PackageNames;

		public override string ToString()
		{
			return Folder.ToString();
		}

		public FolderLink ExpandFolder(FolderResource folder)
		{
			return Folder.Link.Then(folder);
		}

		public FileLink ExpandFolder(FileResource file)
		{
			return Folder.Link.Then(file);
		}

		public FolderLink ExpandDataFolder(FolderResource folder)
		{
			return DataFolder.Link.Then(folder);
		}

		public FileLink ExpandDataFolder(FileResource file)
		{
			return DataFolder.Link.Then(file);
		}
	}
}