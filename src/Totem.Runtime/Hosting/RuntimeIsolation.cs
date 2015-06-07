using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Topshelf;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// Isolates an instance of the runtime configured by the specified host type
	/// </summary>
	/// <typeparam name="THost">The type of host that configures the isolated runtime</typeparam>
	[Serializable]
	internal sealed class RuntimeIsolation<THost> where THost : IRuntimeHost, new()
	{
		[NonSerialized] private AppDomain _appDomain;
		[NonSerialized] private RuntimeBridge<THost> _bridge;

		internal int Run()
		{
			TopshelfExitCode exitCode;

			do
			{
				exitCode = RunInstance();
			}
			while(_bridge.RestartRequested && exitCode == TopshelfExitCode.Ok);

			return (int) exitCode;
		}

		private TopshelfExitCode RunInstance()
		{
			CreateAppDomain();

			CreateBridge();

			ExecuteHostAssembly();

			return RunBridge();
		}

		private void CreateAppDomain()
		{
			// Do not copy all setup properties from current domain, as this MSDN article indicates they are only used by the .NET runtime host:
			//
			// http://msdn.microsoft.com/en-us/library/c8hk0245.aspx

			var setup = new AppDomainSetup
			{
				ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
				ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
			};

			_appDomain = AppDomain.CreateDomain(typeof(THost).FullName, null, setup);
		}

		private void CreateBridge()
		{
			_bridge = (RuntimeBridge<THost>) _appDomain.CreateInstanceFromAndUnwrap(
				ReadRuntimeAssemblyPath(),
				typeof(RuntimeBridge<THost>).FullName,
				ignoreCase: false,
				bindingAttr: BindingFlags.Default,
				binder: null,
				args: null,
				culture: null,
				activationAttributes: null);
		}

		private string ReadRuntimeAssemblyPath()
		{
			return Path.Combine(_appDomain.SetupInformation.ApplicationBase, "Totem.Runtime.dll");
		}

		private void ExecuteHostAssembly()
		{
			// Enables Assembly.GetEntryAssembly, required by Topshelf and handled by RuntimeHost.Isolate

			_appDomain.ExecuteAssembly(typeof(THost).Assembly.CodeBase);
		}

		private TopshelfExitCode RunBridge()
		{
			return _bridge.Run(new ConsoleWriter());
		}
	}
}