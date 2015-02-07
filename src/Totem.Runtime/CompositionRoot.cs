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

				_areasByType.Add(area.Type, new AreaComposition(this, area));
			}

			Scope = module.Build();

			Track(Scope);
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
				IConnectable connection;

				if(_area.TryResolveConnection(_root.Scope, out connection))
				{
					Track(connection.Connect(this));
				}
			}
		}
	}
}