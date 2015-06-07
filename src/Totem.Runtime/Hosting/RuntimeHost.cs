using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog.Extras.Topshelf;
using Topshelf;
using Topshelf.HostConfigurators;
using Totem.IO;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// Hosts an instance of the Totem runtime
	/// </summary>
	public static class RuntimeHost
	{
		public static IRuntimeBridge Bridge { get; internal set; }

		public static int Isolate<THost>() where THost : IRuntimeHost, new()
		{
			// Assembly.GetEntryAssembly returns null in an explicitly-created app domain.
			//
			// This makes sense, as we do not tell the isolated app domain to execute any code.
			//
			// However, Topshelf depends on it. We have to tell the app domain to execute *something*.
			//
			// So, we execute the host assembly again, a second, benign instance. If there is already
			// a bridge in place, this is that instance, and we should exit without issue.

			if(RuntimeHost.Bridge != null)
			{
				return 0;
			}
			
			return new RuntimeIsolation<THost>().Run();
		}

		public static void UseRuntime(this HostConfigurator configurator, IRuntimeHost host)
		{
			var service = new RuntimeService(host.GetType().Assembly, configurator.ReadDeployLink());

			configurator.UseSerilog(service.Log.Logger);

			configurator.Service(() => service);
		}

		private static IOLink ReadDeployLink(this HostConfigurator configurator)
		{
			string link = null;

			configurator.AddCommandLineDefinition("deploy", value => link = value);

			configurator.ApplyCommandLine();

			return String.IsNullOrEmpty(link) ? null : IOLink.From(link);
		}
	}
}