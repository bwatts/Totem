using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// The current deployment of the Totem runtime
	/// </summary>
	public sealed class RuntimeDeployment
	{
		public RuntimeDeployment(bool inSolution, bool inConsole, IFolder folder, IFolder dataFolder, RuntimeDeploymentLog log, IReadOnlyList<string> packageNames)
		{
			InSolution = inSolution;
			InConsole = inConsole;
			Folder = folder;
			DataFolder = dataFolder;
			Log = log;
			PackageNames = packageNames;
		}

		public bool InSolution { get; private set; }
		public bool InConsole { get; private set; }
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

		public FileLink Expand(FileName file)
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

		public FileLink ExpandInData(FileName file)
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