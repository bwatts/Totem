using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime.Configuration.Deployment
{
	/// <summary>
	/// Configures the paths to copy when deploying the Totem runtime
	/// </summary>
	public class CopyElement : ConfigurationElement
	{
		[ConfigurationProperty("resource", IsRequired = true)]
		public IOResource Resource
		{
			get { return (IOResource) this["resource"]; }
			set { this["resource"] = value; }
		}
	}
}