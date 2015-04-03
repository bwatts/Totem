using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

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

		public ApiType GetApi(RuntimeTypeKey key, bool strict = true)
		{
			return Regions.GetApi(key, strict);
		}

		public ApiType GetApi(Type declaredType, bool strict = true)
		{
			return Regions.GetApi(declaredType, strict);
		}

		public ViewType GetView(RuntimeTypeKey key, bool strict = true)
		{
			return Regions.GetView(key, strict);
		}

		public ViewType GetView(Type declaredType, bool strict = true)
		{
			return Regions.GetView(declaredType, strict);
		}
	}
}