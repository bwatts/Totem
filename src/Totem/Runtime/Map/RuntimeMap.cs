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
		public RuntimeMap(IRuntimeDeployment deployment, RuntimeRegionSet regions)
		{
			Deployment = deployment;
			Regions = regions;

			Catalog = new AggregateCatalog(
				from region in Regions
				from package in region.Packages
				select package.Catalog);
		}

		public readonly IRuntimeDeployment Deployment;
		public readonly RuntimeRegionSet Regions;
		public readonly AggregateCatalog Catalog;

		public RuntimeRegion GetRegion(RuntimeRegionKey key, bool strict = true)
		{
			return Regions.Get(key, strict);
		}

		public RuntimePackage GetPackage(string name, bool strict = true)
		{
			var package = Regions
				.Select(region => region.Packages.Get(name, strict: false))
				.FirstOrDefault(regionPackage => regionPackage != null);

			Expect(strict && package == null).IsFalse("Failed to resolve package", name);

			return package;
		}

		public AreaType GetArea(RuntimeTypeKey key, bool strict = true)
		{
			var region = GetRegion(key.Region, strict);

			return region == null ? null : region.GetArea(key, strict);
		}

		public AreaType GetArea(Type declaredType, bool strict = true)
		{
			var area = Regions
				.Select(region => region.GetArea(declaredType, strict: false))
				.FirstOrDefault(regionArea => regionArea != null);

			Expect(strict && area == null).IsFalse("Failed to resolve area", Text.Of(declaredType));

			return area;
		}
	}
}