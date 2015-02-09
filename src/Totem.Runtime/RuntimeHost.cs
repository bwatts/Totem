using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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

			return _args.Length > 0 ? RunInstaller(settings) : RunMode(settings);
		}

		private void InitializeRuntimeAndLogTraits(RuntimeSection settings)
		{
			var deployment = settings.ReadDeployment();

			Notion.Traits.Runtime.SetDefaultValue(deployment.ReadMap());
			Notion.Traits.Log.SetDefaultValue(deployment.ReadLog());
		}

		private int RunInstaller(RuntimeSection settings)
		{
			if(FirstArgIs("/install"))
			{
				return new RuntimeHostService(settings).Install();
			}
			else if(FirstArgIs("/uninstall"))
			{
				return new RuntimeHostService(settings).Uninstall();
			}
			else
			{
				throw new Exception(Text.Of("Unrecognized arguments: ").WriteSeparatedBy(" ", _args));
			}
		}

		private bool FirstArgIs(string name)
		{
			return _args[0].Equals(name, StringComparison.OrdinalIgnoreCase);
		}

		private int RunMode(RuntimeSection settings)
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

		private int RunConsole(RuntimeSection settings)
		{
			return new RuntimeHostConsole(settings).Run();
		}

		private int RunService(RuntimeSection settings)
		{
			Directory.SetCurrentDirectory(Assembly.GetEntryAssembly().GetDirectoryName());

			var service = new RuntimeHostService(settings);

			ServiceBase.Run(service);

			return service.ExitCode;
		}
	}
}