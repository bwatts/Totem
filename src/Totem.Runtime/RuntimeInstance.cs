using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// A composed instance of the Totem runtime
	/// </summary>
	internal sealed class RuntimeInstance : Notion
	{
		private IDisposable _composition;

		internal bool TryStart(RuntimeSection settings)
		{
			if(Debugger.IsAttached)
			{
				Start(settings);

				return true;
			}
			else
			{
				try
				{
					Start(settings);

					return true;
				}
				catch(Exception error)
				{
					Log.Error(error, "[runtime] Failed to start");

					return false;
				}
			}
		}

		internal bool TryStop()
		{
			if(Debugger.IsAttached)
			{
				Stop();

				return true;
			}
			else
			{
				try
				{
					Stop();

					return true;
				}
				catch(Exception error)
				{
					Log.Error(error, "[runtime] Failed to stop");

					return false;
				}
			}
		}

		private void Start(RuntimeSection settings)
		{
			Log.Info(
				"[runtime] Composing instance " + Text.If(Runtime.Deployment.Context.InSolution, "in solution", "deployed") + " at {Folder}",
				Runtime.Deployment.Context.Folder.Link);

			_composition = new InstanceComposition(settings).Connect();

			Log.Info("[runtime] Instance composed");
		}

		private void Stop()
		{
			Log.Info("[runtime] Stopping instance");

			_composition.Dispose();

			Log.Info("[runtime] Instance stopped");
		}

		private sealed class InstanceComposition : Connection
		{
			private readonly RuntimeSection _settings;

			internal InstanceComposition(RuntimeSection settings)
			{
				_settings = settings;
			}

			protected override void Open()
			{
				var container = CreateContainer();

				Track(container);
				Track(container.GetExportedValue<CompositionRoot>());
			}

			private CompositionContainer CreateContainer()
			{
				var catalog = new AggregateCatalog(
					new AssemblyCatalog(Assembly.GetExecutingAssembly()),
					Runtime.Catalog);
				
				return new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);
			}
		}
	}
}