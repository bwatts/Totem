using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog.Extras.Topshelf;
using Topshelf;
using Topshelf.HostConfigurators;

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
			return new RuntimeIsolation<THost>().Run();
		}

		public static void UseRuntime(this HostConfigurator configurator, IRuntimeHost host)
		{
			var service = new RuntimeService(host.GetType().Assembly);

			configurator.UseSerilog(service.Log.Logger);

			configurator.Service(() => service);
		}
	}
}