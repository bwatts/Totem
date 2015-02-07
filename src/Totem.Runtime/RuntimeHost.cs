using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using Totem.Reflection;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// Hosts an instance of the Totem runtime
	/// </summary>
	public class RuntimeHost
	{
		private readonly string[] _args;

		public RuntimeHost(string[] args)
		{
			_args = args;
		}

		public int Run()
		{
			try
			{
				var settings = RuntimeSection.Read();

				return _args.Length > 0 ? RunInstaller(settings) : RunMode(settings);
			}
			catch(Exception error)
			{
				//Console.WriteLine("Failed to start runtime host: " + Text.Of(error));

				return -1;
			}
		}

		private int RunInstaller(RuntimeSection settings)
		{
			try
			{
				if(FirstArgIs("/install"))
				{
					RuntimeHostService.Install();
				}
				else if(FirstArgIs("/uninstall"))
				{
					RuntimeHostService.Uninstall();
				}
				else
				{
					throw new Exception(Text.Of("Unrecognized arguments: ").WriteSeparatedBy(" ", _args));
				}

				return 0;
			}
			catch(Exception error)
			{
				//Console.WriteLine("Failed to run installer: " + Text.Of(error));

				return -1;
			}
		}

		private bool FirstArgIs(string name)
		{
			return _args[0].Equals(name, StringComparison.OrdinalIgnoreCase);
		}

		private static int RunMode(RuntimeSection settings)
		{
			switch(settings.Mode)
			{
				case RuntimeMode.Console:
					return RunConsole(settings);
				case RuntimeMode.Service:
					return RunService(settings);
				default:
					throw new ConfigurationErrorsException(Text.Of("Unsupported mode: ") + settings.Mode);
			}
		}

		private static int RunConsole(RuntimeSection settings)
		{
			return new RuntimeHostConsole(settings).Run();
		}

		private static int RunService(RuntimeSection settings)
		{
			Directory.SetCurrentDirectory(Assembly.GetEntryAssembly().GetDirectoryName());

			var service = new RuntimeHostService(settings);

			ServiceBase.Run(service);

			return service.ExitCode;
		}
	}
}