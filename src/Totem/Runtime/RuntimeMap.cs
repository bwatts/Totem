using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace Totem.Runtime
{
	/// <summary>
	/// A map of the elements within a Totem runtime
	/// </summary>
	public sealed class RuntimeMap : Notion
	{
		public RuntimeMap(RuntimeDeployment deployment, IReadOnlyList<RuntimeRegion> regions)
		{
			Deployment = deployment;
			Regions = regions;

			Catalog = new AggregateCatalog(
				from region in Regions
				from package in region.Packages
				select package.Catalog);
		}

		public RuntimeDeployment Deployment { get; private set; }
		public IReadOnlyList<RuntimeRegion> Regions { get; private set; }
		public AggregateCatalog Catalog { get; private set; }

		public AreaType GetArea(RuntimeTypeKey key, bool strict = true)
		{
			var area = Regions
				.Select(region => region.GetArea(key, strict: false))
				.FirstOrDefault(regionArea => regionArea != null);

			Expect(strict && area == null).IsFalse("Failed to resolve area", key.ToText());

			return area;
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