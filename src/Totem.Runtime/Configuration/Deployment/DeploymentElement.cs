using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime.Configuration.Deployment
{
	/// <summary>
	/// Configures the deployment of the Totem runtime
	/// </summary>
	public class DeploymentElement : ConfigurationElement
	{
		[ConfigurationProperty("dataFolder", DefaultValue = "Deployment")]
		public FolderResource DataFolder
		{
			get { return (FolderResource) this["dataFolder"]; }
			set { this["dataFolder"] = value; }
		}

		[ConfigurationProperty("packages")]
		public PackagesElement Packages
		{
			get { return (PackagesElement) this["packages"]; }
			set { this["packages"] = value; }
		}

		[ConfigurationProperty("copiedResources")]
		public CopiedResourcesElement CopiedResources
		{
			get { return (CopiedResourcesElement) this["copiedResources"]; }
			set { this["copiedResources"] = value; }
		}
	}
}