using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// Installs a service that hosts the Totem runtime
	/// </summary>
	[RunInstaller(true)]
	public sealed class RuntimeHostServiceInstaller : Installer
	{
		public override void Install(IDictionary stateSaver)
		{
			Installers.Add(ReadServiceProcessInstaller());
			Installers.Add(ReadServiceInstaller());

			base.Install(stateSaver);
		}

		public override void Uninstall(IDictionary savedState)
		{
			Installers.Add(ReadServiceProcessInstaller());
			Installers.Add(ReadServiceInstaller());

			base.Uninstall(savedState);
		}

		private ServiceProcessInstaller ReadServiceProcessInstaller()
		{
			var account = Context.Parameters.ContainsKey("account") ? Context.Parameters["account"] : "";
			var username = Context.Parameters.ContainsKey("username") ? Context.Parameters["username"] : "";
			var password = Context.Parameters.ContainsKey("password") ? Context.Parameters["password"] : "";

			var parsedAccount = ServiceAccount.LocalSystem;

			if(account != "")
			{
				if(Enum.TryParse<ServiceAccount>(account, ignoreCase: true, result: out parsedAccount))
				{
					throw new Exception(Text.Of("Failed to parse '{0}' to {1}", account, typeof(ServiceAccount)));
				}

				var hasUsername = username != "";
				var hasPassword = password != "";

				if(parsedAccount == ServiceAccount.User && (!hasUsername || !hasPassword))
				{
					throw new Exception("Username and password are required with the User account");
				}

				if(parsedAccount != ServiceAccount.User && (hasUsername || hasPassword))
				{
					throw new Exception("Username and password are only relevant to the User account");
				}
			}

			return new ServiceProcessInstaller
			{
				Account = parsedAccount,
				Username = username,
				Password = password
			};
		}

		private ServiceInstaller ReadServiceInstaller()
		{
			var serviceElement = RuntimeSection.Read().Service;

			if(serviceElement.Name == "")
			{
				throw new Exception("<totem.runtime> is missing the <service> element or the name attribute is empty");
			}
			
			return serviceElement.ReadServiceInstaller();
		}
	}
}