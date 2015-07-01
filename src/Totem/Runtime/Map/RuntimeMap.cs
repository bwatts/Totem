using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A map of the elements within a Totem runtime
	/// </summary>
	public sealed class RuntimeMap : Notion
	{
		public RuntimeMap(RuntimeDeployment deployment, RuntimeRegionSet regions)
		{
			Deployment = deployment;
			Regions = regions;

			Catalog = new AggregateCatalog(
				from region in Regions
				from package in region.Packages
				select package.Catalog);
		}

		public readonly RuntimeDeployment Deployment;
		public readonly RuntimeRegionSet Regions;
		public readonly AggregateCatalog Catalog;

		public RuntimeRegion GetRegion(RuntimeRegionKey key, bool strict = true)
		{
			return Regions.Get(key, strict);
		}

		public RuntimePackage GetPackage(string name, bool strict = true)
		{
			return Regions.GetPackage(name, strict);
		}

		public AreaType GetArea(RuntimeTypeKey key, bool strict = true)
		{
			return Regions.GetArea(key, strict);
		}

		public AreaType GetArea(Type declaredType, bool strict = true)
		{
			return Regions.GetArea(declaredType, strict);
		}

		public ViewType GetView(RuntimeTypeKey key, bool strict = true)
		{
			return Regions.GetView(key, strict);
		}

		public ViewType GetView(Type declaredType, bool strict = true)
		{
			return Regions.GetView(declaredType, strict);
		}

		public FlowType GetFlow(RuntimeTypeKey key, bool strict = true)
		{
			return Regions.GetFlow(key, strict);
		}

		public FlowType GetFlow(Type declaredType, bool strict = true)
		{
			return Regions.GetFlow(declaredType, strict);
		}

		public EventType GetEvent(RuntimeTypeKey key, bool strict = true)
		{
			return Regions.GetEvent(key, strict);
		}

		public EventType GetEvent(Type declaredType, bool strict = true)
		{
			return Regions.GetEvent(declaredType, strict);
		}

		public WebApiType GetWebApi(RuntimeTypeKey key, bool strict = true)
		{
			return Regions.GetWebApi(key, strict);
		}

		public WebApiType GetWebApi(Type declaredType, bool strict = true)
		{
			return Regions.GetWebApi(declaredType, strict);
		}
	}
}