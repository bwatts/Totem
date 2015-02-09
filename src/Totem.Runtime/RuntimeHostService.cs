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
	internal sealed class RuntimeHostService : ServiceBase, ITaggable
	{
		private readonly RuntimeInstance _instance = new RuntimeInstance();
		private readonly RuntimeSection _settings;

		public RuntimeHostService(RuntimeSection settings)
		{
			_settings = settings;

			_settings.Service.Configure(this);

			Tags = new Tags();
		}

		public Tags Tags { get; private set; }
		private ILog Log { get { return Notion.Traits.Log.Get(this); } }

		protected override void OnStart(string[] args)
		{
			if(!_instance.TryStart(_settings))
			{
				ExitCode = -1;
			}

			base.OnStart(args);
		}

		protected override void OnStop()
		{
			if(!_instance.TryStop())
			{
				ExitCode = -1;
			}

			base.OnStop();
		}

		//
		// Installation
		//

		public int Install()
		{
			try
			{
				Log.Info("Installing the runtime as a Windows service");

				var installer = new AssemblyInstaller(typeof(RuntimeHost).Assembly, null);
				var savedState = new Hashtable();

				installer.Install(savedState);
				installer.Commit(savedState);

				Log.Info("Installation succeeded");

				return 0;
			}
			catch(Exception error)
			{
				Log.Error("Installation failed", error);

				return -1;
			}
		}

		public int Uninstall()
		{
			try
			{
				Log.Info("Uninstalling the runtime Windows service");

				var installer = new AssemblyInstaller(typeof(RuntimeHost).Assembly, null);
				var savedState = new Hashtable();

				installer.Uninstall(savedState);

				Log.Info("Uninstallation succeeded");

				return 0;
			}
			catch(Exception error)
			{
				Log.Error("Uninstallation failed", error);

				return -1;
			}
		}
	}
}