using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Totem.IO;
using Totem.Reflection;
using Totem.Runtime.Configuration;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
	/// <summary>
	/// Initializes the runtime log and map
	/// </summary>
	internal static class RuntimeInitialization
	{
		internal static ILog ReadLog(this RuntimeDeployment deployment)
		{
			var level = (LogEventLevel) (deployment.LogLevel - 1);

			var configuration = new LoggerConfiguration()
				.MinimumLevel.Is(level)
				.WriteTo.RollingFile(
					deployment.LogFolder.Link.Then(FileResource.From("runtime-{Date}.txt")).ToString(),
					outputTemplate: "{Timestamp:hh:mm:ss.fff tt} {Level,-11} | {Message}{NewLine}{Exception}");

			if(deployment.InConsole)
			{
				configuration = configuration.WriteTo.ColoredConsole(level, outputTemplate: "{Timestamp:hh:mm:ss.fff tt} | {Message}{NewLine}{Exception}");
			}

			if(deployment.LogServerHref != null)
			{
				configuration = configuration.WriteTo.Seq(deployment.LogServerHref.ToString(), level);
			}

			return new SerilogAdapter(configuration.CreateLogger(), deployment.LogLevel);
		}

		internal static RuntimeMap ReadMap(this RuntimeDeployment deployment)
		{
			var map = new RuntimeMap(deployment, deployment.ReadRegions());

			map.RegisterTypes();

			map.RegisterAreaDependencies();

			return map;
		}

		//
		// Regions
		//

		private static RuntimeRegionSet ReadRegions(this RuntimeDeployment deployment)
		{
			return new RuntimeRegionSet(
				from packageName in deployment.PackageNames
				let package = deployment.ReadPackage(packageName)
				group package by package.RegionKey into packagesByRegion
				select new RuntimeRegion(packagesByRegion.Key, new RuntimePackageSet(packagesByRegion)));
		}

		private static RuntimePackage ReadPackage(this RuntimeDeployment deployment, string name)
		{
			var folder = deployment.ReadPackageFolder(name);

			var catalog = deployment.ReadPackageCatalog(name, folder);

			return new RuntimePackage(name, folder, catalog.ReadRegionKey(), catalog);
		}

		private static FolderLink ReadPackageFolder(this RuntimeDeployment deployment, string name)
		{
			// TODO: Ensure that Totem.* is a package when deployed

			if(!deployment.InSolution || !name.StartsWith("Totem."))
			{
				return deployment.Expand(FolderResource.From(name));
			}

			var solutionFolder = deployment.Folder.Link.Up(1);

			var submoduleFolder = FolderResource.From("submodules/Totem/src").Then(FolderResource.From(name));

			return solutionFolder.Then(submoduleFolder);
		}

		private static AssemblyCatalog ReadPackageCatalog(this RuntimeDeployment deployment, string name, FolderLink folder)
		{
			if(deployment.InSolution)
			{
				folder = folder.Then(FolderResource.From("bin/" + RuntimeDeployment.BuildType.ToString()));
			}

			return new AssemblyCatalog(folder.Then(FileName.From(name, "dll")).ToString());
		}

		private static RuntimeRegionKey ReadRegionKey(this AssemblyCatalog catalog)
		{
			var attribute = catalog == null ? null : catalog.Assembly.GetCustomAttribute<RuntimePackageAttribute>();

			var regionKey = attribute == null ? null : attribute.Region;

			Expect.That(regionKey).IsNotNull("Assembly is not a runtime package", expected: "Decorated with " + Text.OfType<RuntimePackageAttribute>());

			return regionKey;
		}

		//
		// Types
		//

		private static void RegisterTypes(this RuntimeMap map)
		{
			foreach(var type in
				from region in map.Regions
				from package in region.Packages
				from declaredType in package.Assembly.GetTypes()
				where typeof(IRuntimeArea).IsAssignableFrom(declaredType)
					&& declaredType.IsPublic
					&& declaredType.IsClass
					&& !declaredType.IsAbstract
					&& !declaredType.IsAnonymous()
				select new { package, declaredType })
			{
				map.TryReadArea(type.package, type.declaredType);
			}
		}

		private static RuntimeTypeRef ReadType(this RuntimePackage package, Type declaredType)
		{
			return new RuntimeTypeRef(package, declaredType, new RuntimeState(declaredType));
		}

		private static void TryReadArea(this RuntimeMap map, RuntimePackage package, Type declaredType)
		{
			if(typeof(IRuntimeArea).IsAssignableFrom(declaredType))
			{
				package.Areas.Register(new AreaType(
					package.ReadType(declaredType),
					declaredType.ReadSectionName()));
			}
		}

		private static string ReadSectionName(this Type areaType)
		{
			var attribute = areaType.GetCustomAttribute<ConfigurationSectionAttribute>();

			return attribute == null ? "" : attribute.Name;
		}

		private static void RegisterAreaDependencies(this RuntimeMap map)
		{
			foreach(var areaDependency in
				from region in map.Regions
				from package in region.Packages
				from area in package.Areas
				from dependsOnAttribute in area.DeclaredType.GetCustomAttributes<DependsOnAttribute>(inherit: true)
				select new { area, Dependency = map.GetArea(dependsOnAttribute.AreaType) })
			{
				areaDependency.area.Dependencies.Register(areaDependency.Dependency);
			}
		}
	}
}