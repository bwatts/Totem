using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Totem.IO;
using Totem.Runtime.Configuration.Console;

namespace Totem.Runtime.Configuration
{
	/// <summary>
	/// Configures the current Totem runtime
	/// </summary>
	public class RuntimeSection : ConfigurationSection
	{
		[ConfigurationProperty("dataFolder", IsRequired = true)]
		public string DataFolder
		{
			get { return (string) this["dataFolder"]; }
			set { this["dataFolder"] = value; }
		}

		[ConfigurationProperty("log", IsRequired = true)]
		public LogElement Log
		{
			get { return (LogElement) this["log"]; }
			set { this["log"] = value; }
		}

		[ConfigurationProperty("console")]
		public ConsoleElement Console
		{
			get { return (ConsoleElement) this["console"]; }
			set { this["console"] = value; }
		}

		public bool HasUI => Environment.UserInteractive;

		//
		// Map
		//

		public RuntimeDeployment ReadDeployment(Assembly programAssembly)
		{
			var hostFolder = GetHostFolder();

			SetCurrentDirectory(hostFolder);

			string solutionConfiguration;

			var inSolution = TryGetSolutionConfiguration(hostFolder, out solutionConfiguration);

			// Go up past bin/%solutionConfiguration% when in the solution

			var folder = hostFolder.Up(inSolution ? 3 : 1);

			var dataFolder = GetDataFolder(folder);

			var logFolder = GetLogFolder(dataFolder);

			var hostExe = FileLink.From(programAssembly.Location);

			return new RuntimeDeployment(folder, hostFolder, dataFolder, logFolder, hostExe, solutionConfiguration);
		}

		private static IFolder GetHostFolder()
		{
			return new LocalFolder(FolderLink.From(AppDomain.CurrentDomain.BaseDirectory));
		}

		private static void SetCurrentDirectory(IFolder folder)
		{
			Directory.SetCurrentDirectory(folder.Link.ToString());
		}

		private static bool TryGetSolutionConfiguration(IFolder hostFolder, out string solutionConfiguration)
		{
			var hostSegments = hostFolder.Link.Resource.Path.Segments;

			if(hostSegments.Count > 2
				&& hostSegments[hostSegments.Count - 2].ToString().Equals("bin", StringComparison.OrdinalIgnoreCase))
			{
				solutionConfiguration = hostSegments[hostSegments.Count - 1].ToString();

				return true;
			}

			solutionConfiguration = "";

			return false;
		}

		private IFolder GetDataFolder(IFolder deploymentFolder)
		{
			var dataFolder = Path.IsPathRooted(DataFolder)
				? DataFolder
				: Path.GetFullPath(Path.Combine(deploymentFolder.Link.ToString(), DataFolder));

			return new LocalFolder(FolderLink.From(dataFolder));
		}

		private IFolder GetLogFolder(IFolder dataFolder)
		{
			return dataFolder.Then(FolderResource.From(Log.DataFolder));
		}

		//
		// Factory
		//

		public static RuntimeSection Read(bool strict = true)
		{
			var section = (RuntimeSection) ConfigurationManager.GetSection("totem.runtime");

			Expect.False(strict && section == null, @"Runtime is not configured. Specify this in the configuration file:

<configSections>
	<section name=" + "\"totem.runtime\" type=\"Totem.Runtime.Configuration.RuntimeSection, Totem.Runtime\"" + @" />
</configSections>
");

			return section;
		}
	}
}