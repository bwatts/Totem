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
		public static void UseRuntime(this HostConfigurator configurator, Assembly hostAssembly)
		{
			var service = new RuntimeService(hostAssembly, configurator.ReadDeployLink());

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