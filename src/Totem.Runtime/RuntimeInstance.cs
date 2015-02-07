using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// A composed instance of the Totem runtime
	/// </summary>
	internal sealed class RuntimeInstance
	{
		private IDisposable _composition;

		internal Exception Error { get; private set; }
		internal bool Debugging { get { return Debugger.IsAttached; } }

		internal bool TryStart(RuntimeSection settings)
		{
			return TryTransition(() => ComposeInstance(settings));
		}

		internal bool TryStop()
		{
			return TryTransition(DisposeInstance);
		}

		private bool TryTransition(Action transition)
		{
			if(Debugging)
			{
				transition();

				return true;
			}
			else
			{
				Error = null;

				try
				{
					transition();
				}
				catch(Exception error)
				{
					Error = error;
				}

				return Error == null;
			}
		}

		private void ComposeInstance(RuntimeSection settings)
		{
			_composition = new InstanceComposition(settings).Connect();
		}

		private void DisposeInstance()
		{
			if(_composition != null)
			{
				_composition.Dispose();
			}
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
				SetRuntimeTrait();

				Compose();
			}

			private void SetRuntimeTrait()
			{
				Notion.Traits.Runtime.SetDefaultValue(_settings.ReadMap());
			}

			private void Compose()
			{
				var container = new CompositionContainer(Runtime.Catalog, CompositionOptions.DisableSilentRejection);

				Track(container);

				var root = container.GetExportedValue<CompositionRoot>();

				Track(root.Connect(this));
			}
		}
	}
}