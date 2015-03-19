using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Topshelf;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// The Topshelf service control hosting the Totem runtime
	/// </summary>
	internal sealed class RuntimeService : ServiceControl
	{
		private readonly RuntimeSection _settings;
		private IDisposable _composition;

		internal RuntimeService(RuntimeSection settings)
		{
			_settings = settings;
		}

		public bool Start(HostControl hostControl)
		{
			Task.Run(() =>
			{
				_composition = new InstanceComposition(_settings).Connect();
			});

			return true;
		}

		public bool Stop(HostControl hostControl)
		{
			if(_composition != null)
			{
				_composition.Dispose();
				_composition = null;
			}

			return true;
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