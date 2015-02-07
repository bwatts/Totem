using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Totem.IO;

namespace Totem.Runtime
{
	/// <summary>
	/// Reflects the elements of the Totem domain
	/// </summary>
	internal static class DomainReflection
	{
		internal static RuntimeMap ReadMap(this RuntimeDeployment deployment)
		{
			var map = new RuntimeMap(deployment, deployment.ReadRegions().ToList());

			map.RegisterAreaDependencies();

			return map;
		}

		private static IEnumerable<RuntimeRegion> ReadRegions(this RuntimeDeployment deployment)
		{
			return
				from packageName in deployment.PackageNames
				let package = deployment.ReadPackage(packageName)
				group package by package.RegionKey into packagesByRegionKey
				select new RuntimeRegion(packagesByRegionKey.Key, packagesByRegionKey.ToList());
		}

		private static RuntimePackage ReadPackage(this RuntimeDeployment deployment, string name)
		{
			var folderLink = deployment.Expand(FolderResource.From(name));

			var assemblyLink = folderLink.Then(FileName.From(name, "dll"));

			var catalog = new AssemblyCatalog(assemblyLink.ToString());

			var package = new RuntimePackage(folderLink, catalog.ReadRegionKey(), catalog);

			package.RegisterAreas();

			return package;
		}

		private static RuntimeRegionKey ReadRegionKey(this AssemblyCatalog catalog)
		{
			var attribute = catalog == null ? null : catalog.Assembly.GetCustomAttribute<RuntimePackageAttribute>();

			var regionKey = attribute == null ? null : attribute.Region;

			Expect.That(regionKey).IsNotNull("Assembly is not a runtime package", expected: "Decorated with " + Text.OfType<RuntimePackageAttribute>());

			return regionKey;
		}

		private static void RegisterAreas(this RuntimePackage package)
		{
			// Principles here are borrowed from those of MEF metadata, the implementation of which proved to be troublesome.
			//
			// The friction is due to our exposure of a static runtime map, where MEF promotes a dynamic structure.
			//
			// MEF discovers area definitions, creates instances, and fulfills exports; we establish a middle ground with implicit metadata we read directly.

			foreach(var area in
				from type in package.Assembly.GetTypes()
				where typeof(IRuntimeArea).IsAssignableFrom(type)
				select new AreaType(package, type, type.ReadSectionName()))
			{
				package.Areas.Register(area);
			}
		}

		private static void RegisterAreaDependencies(this RuntimeMap map)
		{
			var areaDependencies =
				from region in map.Regions
				from package in region.Packages
				from area in package.Areas
				from dependsOnAttribute in area.DeclaredType.GetCustomAttributes<DependsOnAttribute>(inherit: true)
				select new
				{
					Area = area,
					Dependency = map.GetArea(dependsOnAttribute.AreaType)
				};

			foreach(var areaDependency in areaDependencies)
			{
				areaDependency.Area.Dependencies.Register(areaDependency.Dependency);
			}
		}

		private static string ReadSectionName(this Type areaType)
		{
			var attribute = areaType.GetCustomAttribute<ConfigurationSectionAttribute>();

			return attribute == null ? "" : attribute.Name;
		}
	}
}