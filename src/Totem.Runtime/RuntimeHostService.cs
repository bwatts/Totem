using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// A Windows Service that hosts the Totem runtime
	/// </summary>
	internal sealed class RuntimeHostService : ServiceBase
	{
		private readonly RuntimeInstance _instance = new RuntimeInstance();
		private readonly RuntimeSection _settings;

		public RuntimeHostService(RuntimeSection settings)
		{
			_settings = settings;

			_settings.Service.Configure(this);
		}

		protected override void OnStart(string[] args)
		{
			if(!_instance.TryStart(_settings))
			{
				ThrowHostException("Runtime failed to start");
			}

			base.OnStart(args);
		}

		protected override void OnStop()
		{
			if(!_instance.TryStop())
			{
				ThrowHostException("Runtime failed to stop");
			}

			base.OnStop();
		}

		private void ThrowHostException(Text header)
		{
			ExitCode = -1;

			// Put the whole exception in the event log, whose details are normally culled

			throw new Exception(header
				.WriteTwoLines()
				.Write("Exception:")
				.WriteTwoLines()
				.Write(_instance.Error));
		}

		//
		// Installation
		//

		public static void Install()
		{
			var installer = new AssemblyInstaller(typeof(RuntimeHost).Assembly, null);
			var savedState = new Hashtable();

			installer.Install(savedState);
			installer.Commit(savedState);
		}

		public static void Uninstall()
		{
			var installer = new AssemblyInstaller(typeof(RuntimeHost).Assembly, null);
			var savedState = new Hashtable();

			installer.Uninstall(savedState);
		}
	}
}