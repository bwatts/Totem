using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Http;
using Totem.IO;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
	/// <summary>
	/// A context-bound instance of the Totem runtime
	/// </summary>
	public class RuntimeDeployment
	{
		public RuntimeDeployment(
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

		public FolderLink Expand(FolderResource folder)
		{
			return Folder.Link.Then(folder);
		}

		public FileLink Expand(FileResource file)
		{
			return Folder.Link.Then(file);
		}

		public FolderLink ExpandInData(FolderResource folder)
		{
			return DataFolder.Link.Then(folder);
		}

		public FileLink ExpandInData(FileResource file)
		{
			return DataFolder.Link.Then(file);
		}

		// TODO: There has to be a better way; other solution configurations would fail.

		public static string BuildType
		{
			get
			{
#if DEBUG
				return "Debug";
#else
				return "Release";
#endif
			}
		}
	}
}