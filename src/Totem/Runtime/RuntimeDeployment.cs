using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime
{
	/// <summary>
	/// The current deployment of the Totem runtime
	/// </summary>
	public sealed class RuntimeDeployment
	{
		public RuntimeDeployment(RuntimeMode mode, bool inSolution, IFolder folder, IFolder dataFolder, RuntimeDeploymentLog log, IReadOnlyList<string> packageNames)
		{
			Mode = mode;
			InSolution = inSolution;
			Folder = folder;
			DataFolder = dataFolder;
			Log = log;
			PackageNames = packageNames;
		}

		public RuntimeMode Mode { get; private set; }
		public bool InSolution { get; private set; }
		public IFolder Folder { get; private set; }
		public IFolder DataFolder { get; private set; }
		public RuntimeDeploymentLog Log { get; private set; }
		public IReadOnlyList<string> PackageNames { get; private set; }

		public override string ToString()
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