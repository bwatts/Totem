using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;

namespace Totem.Runtime.Configuration.Service
{
	/// <summary>
	/// Configures the process hosting the runtime as a Windows Service
	/// </summary>
	public class ProcessElement : ConfigurationElement
	{
		[ConfigurationProperty("account", DefaultValue = ServiceAccount.LocalService)]
		public ServiceAccount Account
		{
			get { return (ServiceAccount) this["account"]; }
			set { this["account"] = value; }
		}

		[ConfigurationProperty("username", DefaultValue = "")]
		public string Username
		{
			get { return (string) this["username"]; }
			set { this["username"] = value; }
		}

		[ConfigurationProperty("password", DefaultValue = "")]
		public string Password
		{
			get { return (string) this["password"]; }
			set { this["password"] = value; }
		}
	}
}