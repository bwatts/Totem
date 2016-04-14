using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Totem.Runtime.Configuration
{
	/// <summary>
	/// Configures the Totem runtime log
	/// </summary>
	public class LogElement : ConfigurationElement
	{
		[ConfigurationProperty("level", DefaultValue = LogLevel.Info)]
		public LogLevel Level
		{
			get { return (LogLevel) this["level"]; }
			set { this["level"] = value; }
		}

		[ConfigurationProperty("dataFolder", IsRequired = true)]
		public string DataFolder
		{
			get { return (string) this["dataFolder"]; }
			set { this["dataFolder"] = value; }
		}
	}
}