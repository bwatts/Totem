using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Totem.Runtime.Configuration.Service
{
	/// <summary>
	/// Configures the lifecycle the runtime as a Windows Service
	/// </summary>
	public class LifecycleElement : ConfigurationElement
	{
		[ConfigurationProperty("canHandlePowerEvent", DefaultValue = false)]
		public bool CanHandlePowerEvent
		{
			get { return (bool) this["canHandlePowerEvent"]; }
			set { this["canHandlePowerEvent"] = value; }
		}

		[ConfigurationProperty("canHandleSessionChangeEvent", DefaultValue = false)]
		public bool CanHandleSessionChangeEvent
		{
			get { return (bool) this["canHandleSessionChangeEvent"]; }
			set { this["canHandleSessionChangeEvent"] = value; }
		}

		[ConfigurationProperty("canPauseAndContinue", DefaultValue = false)]
		public bool CanPauseAndContinue
		{
			get { return (bool) this["canPauseAndContinue"]; }
			set { this["canPauseAndContinue"] = value; }
		}

		[ConfigurationProperty("canShutdown", DefaultValue = false)]
		public bool CanShutdown
		{
			get { return (bool) this["canShutdown"]; }
			set { this["canShutdown"] = value; }
		}

		[ConfigurationProperty("canStop", DefaultValue = true)]
		public bool CanStop
		{
			get { return (bool) this["canStop"]; }
			set { this["canStop"] = value; }
		}
	}
}