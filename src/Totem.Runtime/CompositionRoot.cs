using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace Totem.Runtime
{
	/// <summary>
	/// The point of composition for the Totem runtime
	/// </summary>
	[Export]
	public sealed class CompositionRoot : Connection
	{
		private Dictionary<AreaType, AreaComposition> _areasByType;

		[ImportMany]
		public IRuntimeArea[] Areas { get; set; }

		public ILifetimeScope Scope { get; private set; }

		protected override void Open()
		{
			_areasByType = new Dictionary<AreaType, AreaComposition>();

			try
			{
				ComposeScope();

				Track(ComposeAreas());
			}
			finally
			{
				_areasByType = null;
			}
		}

		protected override void Close()
		{
			Scope = null;

			Log.Info("Closed runtime scope");
		}

		//
		// Composition
		//

		private void ComposeScope()
		{
			var module = new BuilderModule();

			module.RegisterInstance(this).ExternallyOwned();

			foreach(var area in Areas)
			{
				module.RegisterModule(area);

				AddArea(area);
			}

			Scope = module.Build();

			Track(Scope);

			Log.Info("Opened runtime scope");
		}

		private void AddArea(IRuntimeArea area)
		{
			_areasByType.Add(area.Type, new AreaComposition(this, area));
		}

		private IDisposable ComposeAreas()
		{
			return _areasByType.Values.Connect(area => area.GetDependencies(), this);
		}

		private sealed class AreaComposition : Connection
		{
			private readonly CompositionRoot _root;
			private readonly IRuntimeArea _area;

			internal AreaComposition(CompositionRoot root, IRuntimeArea area)
			{
				_root = root;
				_area = area;
			}

			internal IEnumerable<AreaComposition> GetDependencies()
			{
				return _area.Type.Dependencies.Select(dependency => _root._areasByType[dependency]);
			}

			protected override void Open()
			{
				var connection = _area.ResolveConnectionOrNull(_root.Scope);

				if(connection != null)
				{
					Track(connection.Connect(this));
				}

				Log.Info("Started | " + _area.Type.Key.ToText().InBrackets());
			}

			protected override void Close()
			{
				Log.Info("Stopped | " + _area.Type.Key.ToText().InBrackets());
			}
		}
	}
}