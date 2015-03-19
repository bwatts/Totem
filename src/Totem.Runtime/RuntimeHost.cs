using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Extras.Topshelf;
using Topshelf;
using Topshelf.HostConfigurators;
using Totem.IO;
using Totem.Reflection;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// Hosts an instance of the Totem runtime
	/// </summary>
	public static class RuntimeHost
	{
		public static int Run(Action<HostConfigurator> configureHost, string sectionName = "totem.runtime")
		{
			return (int) HostFactory.Run(host =>
			{
				var settings = ReadSettings(sectionName);

				host.UseRuntime(settings);

				configureHost(host);
			});
		}

		private static RuntimeSection ReadSettings(string sectionName)
		{
			var settings = ConfigurationManager.GetSection(sectionName) as RuntimeSection;

			Expect.That(settings).IsNotNull("Configuration section not found: " + sectionName);

			return settings;
		}

		private static void UseRuntime(this HostConfigurator host, RuntimeSection settings)
		{
			SetCurrentDirectory();

			host.Service<RuntimeService>(() => new RuntimeService(settings));

			host.ConfigureRuntime(settings);
		}

		private static void SetCurrentDirectory()
		{
			Directory.SetCurrentDirectory(Assembly.GetEntryAssembly().GetDirectoryName());
		}

		private static void ConfigureRuntime(this HostConfigurator host, RuntimeSection settings)
		{
			var deployment = settings.ReadDeployment();

			var map = deployment.ReadMap();
			var log = deployment.ReadLog();

			Notion.Traits.Runtime.SetDefaultValue(map);
			Notion.Traits.InitializeLog(log);

			log.Info(settings.Deployment.DataFolder.ToText());

			host.UseSerilog(log.Logger);
		}
	}
}