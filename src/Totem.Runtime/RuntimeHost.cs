using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using NDesk.Options;
using Totem.Reflection;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// Hosts an instance of the Totem runtime
	/// </summary>
	public class RuntimeHost : Notion
	{
		private readonly string[] _args;

		public RuntimeHost(string[] args)
		{
			_args = args;
		}

		public int Run()
		{
			if(Debugger.IsAttached)
			{
				RunHost();

				return 0;
			}
			else
			{
				try
				{
					return RunHost();
				}
				catch(Exception error)
				{
					Console.WriteLine(error);

					return -1;
				}
			}
		}

		private int RunHost()
		{
			var settings = RuntimeSection.Read();

			InitializeRuntimeAndLogTraits(settings);

			return RunMode(settings);
		}

		private void InitializeRuntimeAndLogTraits(RuntimeSection settings)
		{
			var deployment = settings.ReadDeployment();

			Notion.Traits.Runtime.SetDefaultValue(deployment.ReadMap());

			Notion.Traits.InitializeLog(deployment.ReadLog());
		}

		private int RunMode(RuntimeSection settings)
		{
			var install = false;
			var uninstall = false;
			var deploy = false;

			var options = new OptionSet
			{
				{ "i|install", x => install = true },
				{ "u|uninstall", x => uninstall = true }
			};

			var modeArgs = options.Parse(_args).ToArray();

			if(install)
			{
				return InstallService(settings, modeArgs);
			}
			else if(uninstall)
			{
				return UninstallService(settings, modeArgs);
			}
			else if(deploy)
			{
				return Deploy(settings, modeArgs);
			}
			else if(settings.Service.IsConfigured)
			{
				return RunService(settings, modeArgs);
			}
			else
			{
				return RunConsole(settings, modeArgs);
			}
		}

		private int InstallService(RuntimeSection settings, string[] args)
		{
			return new RuntimeHostService(settings, args).Install();
		}

		private int UninstallService(RuntimeSection settings, string[] args)
		{
			return new RuntimeHostService(settings, args).Uninstall();
		}

		private int Deploy(RuntimeSection settings, string[] args)
		{
			return new RuntimeHostDeployment(settings, args).Run();
		}

		private int RunService(RuntimeSection settings, string[] args)
		{
			Directory.SetCurrentDirectory(Assembly.GetEntryAssembly().GetDirectoryName());

			var service = new RuntimeHostService(settings, args);

			ServiceBase.Run(service);

			return service.ExitCode;
		}

		private int RunConsole(RuntimeSection settings, string[] args)
		{
			return new RuntimeHostConsole(settings).Run();
		}
	}
}