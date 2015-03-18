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
	/// A Windows Service hosting an instance of the Totem runtime
	/// </summary>
	internal sealed class RuntimeHostService : ServiceBase, ITaggable
	{
		private readonly RuntimeInstance _instance = new RuntimeInstance();
		private readonly RuntimeSection _settings;
		private readonly string[] _args;

		public RuntimeHostService(RuntimeSection settings, string[] args)
		{
			_settings = settings;
			_args = args;
			Tags = new Tags();

			_settings.Service.Configure(this);
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

				Apply((installer, savedState) =>
				{
					installer.Install(savedState);
					installer.Commit(savedState);
				});

				Log.Info("Installation succeeded");

				return 0;
			}
			catch(Exception error)
			{
				Log.Error(error, "Installation failed");

				return -1;
			}
		}

		public int Uninstall()
		{
			try
			{
				Log.Info("Uninstalling the runtime Windows service");

				Apply((installer, savedState) => installer.Uninstall(savedState));

				Log.Info("Uninstallation succeeded");

				return 0;
			}
			catch(Exception error)
			{
				Log.Error(error, "Uninstallation failed");

				return -1;
			}
		}

		internal void Apply(Action<AssemblyInstaller, IDictionary> action)
		{
			var installer = new AssemblyInstaller(GetType().Assembly, _args);
			var savedState = new Hashtable();

			action(installer, savedState);
		}
	}
}