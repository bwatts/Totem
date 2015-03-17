using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace Totem.Runtime.Configuration.Service
{
	/// <summary>
	/// Configures the Windows Service hosting the Totem runtime
	/// </summary>
	public class ServiceElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get { return (string) this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty("description", DefaultValue = "")]
		public string Description
		{
			get { return (string) this["description"]; }
			set { this["description"] = value; }
		}

		[ConfigurationProperty("start", IsRequired = false)]
		public StartElement Start
		{
			get { return (StartElement) this["start"]; }
			set { this["start"] = value; }
		}

		[ConfigurationProperty("lifecycle", IsRequired = false)]
		public LifecycleElement Lifecycle
		{
			get { return (LifecycleElement) this["lifecycle"]; }
			set { this["lifecycle"] = value; }
		}

		public bool IsConfigured { get { return Name != ""; } }

		public ServiceInstaller ReadServiceInstaller()
		{
			return new ServiceInstaller
			{
				ServiceName = Name,
				StartType = Start.Mode,
				DelayedAutoStart = Start.DelayedAutoStart
			};
		}

		public void Configure(ServiceBase service)
		{
			service.ServiceName = Name;
			service.AutoLog = Start.AutoLog;
			service.CanHandlePowerEvent = Lifecycle.CanHandlePowerEvent;
			service.CanHandleSessionChangeEvent = Lifecycle.CanHandleSessionChangeEvent;
			service.CanPauseAndContinue = Lifecycle.CanPauseAndContinue;
			service.CanShutdown = Lifecycle.CanShutdown;
			service.CanStop = Lifecycle.CanStop;
		}
	}
}