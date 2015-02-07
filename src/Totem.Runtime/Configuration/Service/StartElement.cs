using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;

namespace Totem.Runtime.Configuration.Service
{
	/// <summary>
	/// Configures startup when hosting the runtime as a Windows Service
	/// </summary>
	public class StartElement : ConfigurationElement
	{
		[ConfigurationProperty("mode", DefaultValue = "Automatic")]
		public ServiceStartMode Mode
		{
			get { return (ServiceStartMode) this["mode"]; }
			set { this["mode"] = value; }
		}

		[ConfigurationProperty("autoLog", DefaultValue = true)]
		public bool AutoLog
		{
			get { return (bool) this["autoLog"]; }
			set { this["autoLog"] = value; }
		}

		[ConfigurationProperty("delayedAutoStart", DefaultValue = false)]
		public bool DelayedAutoStart
		{
			get { return (bool) this["delayedAutoStart"]; }
			set { this["delayedAutoStart"] = value; }
		}
	}
}