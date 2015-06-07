using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Totem.IO;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of related aseemblies in a region of a Totem runtime
	/// </summary>
	public sealed class RuntimePackage : Notion
	{
		public RuntimePackage(string name, IFolder buildFolder, IFolder deploymentFolder, AssemblyCatalog catalog, RuntimeRegionKey regionKey)
		{
			Name = name;
			BuildFolder = buildFolder;
			DeploymentFolder = deploymentFolder;
			Catalog = catalog;
			RegionKey = regionKey;
			Assembly = catalog.Assembly;
			Areas = new AreaTypeSet();
			Views = new ViewTypeSet();
			Flows = new FlowTypeSet();
			Events = new EventTypeSet();
		}

		public readonly string Name;
		public readonly IFolder BuildFolder;
		public readonly IFolder DeploymentFolder;
		public readonly AssemblyCatalog Catalog;
		public readonly RuntimeRegionKey RegionKey;
		public readonly Assembly Assembly;
		public readonly AreaTypeSet Areas;
		public readonly ViewTypeSet Views;
		public readonly FlowTypeSet Flows;
		public readonly EventTypeSet Events;

		public override Text ToText()
		{
			return Name;
		}

		public AreaType GetArea(RuntimeTypeKey key, bool strict = true)
		{
			return Areas.Get(key, strict);
		}

		public AreaType GetArea(Type declaredType, bool strict = true)
		{
			return Areas.Get(declaredType, strict);
		}

		public ViewType GetView(RuntimeTypeKey key, bool strict = true)
		{
			return Views.Get(key, strict);
		}

		public ViewType GetView(Type declaredType, bool strict = true)
		{
			return Views.Get(declaredType, strict);
		}

		public FlowType GetFlow(RuntimeTypeKey key, bool strict = true)
		{
			return Flows.Get(key, strict);
		}

		public FlowType GetFlow(Type declaredType, bool strict = true)
		{
			return Flows.Get(declaredType, strict);
		}

		public EventType GetEvent(RuntimeTypeKey key, bool strict = true)
		{
			return Events.Get(key, strict);
		}

		public EventType GetEvent(Type declaredType, bool strict = true)
		{
			return Events.Get(declaredType, strict);
		}
	}
}