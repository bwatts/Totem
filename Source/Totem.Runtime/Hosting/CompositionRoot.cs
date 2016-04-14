using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Autofac;
using Totem.Runtime.Map;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// The point of composition for the Totem runtime
	/// </summary>
	[Export]
	[PartCreationPolicy(CreationPolicy.Shared)]
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

			Log.Debug("[runtime] Closed root composition scope");
		}

		//
		// Composition
		//

		private void ComposeScope()
		{
			Scope = BuildScope();

			Track(Scope);

			Log.Debug("[runtime] Opened root composition scope");
		}

		private ILifetimeScope BuildScope()
		{
			var module = new BuilderModule();

			RegisterContext(module);

			RegisterAreas(module);

			return module.Build();
		}

		private void RegisterContext(BuilderModule module)
		{
			module.RegisterInstance(this).ExternallyOwned();

			module.RegisterInstance(RuntimeService.Instance).ExternallyOwned();

			module.RegisterType<DependencySource>().As<IDependencySource>().InstancePerDependency();
		}

		private void RegisterAreas(BuilderModule module)
		{
			foreach(var area in Areas)
			{
				module.RegisterModule(area);

				AddArea(area);
			}
		}

		private void AddArea(IRuntimeArea area)
		{
			_areasByType.Add(area.AreaType, new AreaComposition(this, area));
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
				return _area.AreaType.Dependencies.Values.Select(dependency => _root._areasByType[dependency]);
			}

			protected override void Open()
			{
				Track(_area.Compose(_root.Scope));

				Log.Info("[runtime] Started {Area:l}", _area.AreaType);
			}

			protected override void Close()
			{
				Log.Info("[runtime] Stopped {Area:l}", _area.AreaType);
			}
		}
	}
}