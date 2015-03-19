using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Totem.IO;
using Totem.Runtime.Configuration.Console;
using Totem.Runtime.Configuration.Deployment;
using Totem.Runtime.Configuration.Service;
using Totem.Runtime.Map;

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

		[ConfigurationProperty("service")]
		public ServiceElement Service
		{
			get { return (ServiceElement) this["service"]; }
			set { this["service"] = value; }
		}

		[ConfigurationProperty("deployment", IsRequired = true)]
		public DeploymentElement Deployment
		{
			get { return (DeploymentElement) this["deployment"]; }
			set { this["deployment"] = value; }
		}

		public bool UserInteractive { get { return Environment.UserInteractive; } }

		//
		// Map
		//

		public RuntimeDeployment ReadDeployment()
		{
			var hostFolder = GetHostFolder();

			var inSolution = InSolution(hostFolder);

			var deploymentFolder = GetDeploymentFolder(hostFolder, inSolution);

			var dataFolder = GetDataFolder(deploymentFolder);

			var logFolder = GetLogFolder(dataFolder);

			return new RuntimeDeployment(new RuntimeContext(
				inSolution,
				UserInteractive,
				deploymentFolder,
				dataFolder,
				Log.Level,
				new LocalFolder(logFolder),
				Deployment.Packages.GetNames()));
		}

		private static IFolder GetHostFolder()
		{
			return new LocalFolder(FolderLink.From(Directory.GetCurrentDirectory()));
		}

		private static bool InSolution(IFolder hostFolder)
		{
			var segments = hostFolder.Link.Resource.Path.Segments;

			return segments.Count > 2
				&& segments[segments.Count - 1].ToString().Equals(RuntimeDeployment.BuildType, StringComparison.OrdinalIgnoreCase)
				&& segments[segments.Count - 2].ToString().Equals("bin", StringComparison.OrdinalIgnoreCase);
		}

		private static IFolder GetDeploymentFolder(IFolder hostFolder, bool inSolution)
		{
			// Omit bin/%BuildType% if not deployed

			return new LocalFolder(hostFolder.Link.Up(inSolution ? 3 : 1));
		}

		private IFolder GetDataFolder(IFolder deploymentFolder)
		{
			var dataFolder = Path.IsPathRooted(DataFolder)
				? DataFolder
				: Path.GetFullPath(Path.Combine(deploymentFolder.Link.ToString(), DataFolder));

			return new LocalFolder(FolderLink.From(dataFolder));
		}

		private FolderLink GetLogFolder(IFolder dataFolder)
		{
			return dataFolder.Link.Then(FolderResource.From(Log.DataFolder));
		}

		//
		// Factory
		//

		public static RuntimeSection Read()
		{
			var instance = (RuntimeSection) ConfigurationManager.GetSection("totem.runtime");

			Expect.That(instance).IsNotNull("Runtime is not configured - specify the 'totem.runtime' section in the configuration file");

			return instance;
		}
	}
}