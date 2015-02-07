using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime
{
	/// <summary>
	/// The current deployment of the Totem runtime
	/// </summary>
	public sealed class RuntimeDeployment : Notion
	{
		public RuntimeDeployment(bool inSolution, IFolder folder, IFolder dataFolder, IReadOnlyList<string> packageNames)
		{
			InSolution = inSolution;
			Folder = folder;
			DataFolder = dataFolder;
			PackageNames = packageNames;
		}

		public bool InSolution { get; private set; }
		public IFolder Folder { get; private set; }
		public IFolder DataFolder { get; private set; }
		public IReadOnlyList<string> PackageNames { get; private set; }

		public override Text ToText()
		{
			return Folder.ToText();
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