using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;
using Totem.IO;

namespace Totem.Runtime
{
	/// <summary>
	/// Initializes the map and log of the Totem runtime
	/// </summary>
	internal static class RuntimeInitialization
	{
		//
		// Map
		//

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
			var folder = deployment.Expand(FolderResource.From(name));

			var catalog = deployment.ReadPackageCatalog(name, folder);

			var package = new RuntimePackage(folder, catalog.ReadRegionKey(), catalog);

			package.RegisterAreas();

			return package;
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

		//
		// Log
		//

		internal static ILog ReadLog(this RuntimeDeployment deployment)
		{
			var level = (LogEventLevel) (deployment.Log.Level - 1);

			var configuration = new LoggerConfiguration().WriteTo.RollingFile(
				deployment.Log.Folder.Link.Then(FileResource.From("runtime-{Date}.txt")).ToString(),
				level);

			if(deployment.Mode == RuntimeMode.Console)
			{
				configuration = configuration.WriteTo.ColoredConsole(level, outputTemplate: "{Timestamp:h:mm:ss.fff tt} [{Level}] {Message}{NewLine}{Exception}");
			}

			if(deployment.Log.ServerUrl != "")
			{
				configuration = configuration.WriteTo.Seq(deployment.Log.ServerUrl, level);
			}
				
			return new SerilogAdapter(configuration.CreateLogger());
		}

		private sealed class SerilogAdapter : ILog
		{
			private readonly ILogger _logger;

			internal SerilogAdapter(ILogger logger)
			{
				_logger = logger;
			}

			public LogLevel Level { get; private set; }

			public void Write(LogMessage message)
			{
				var level = (LogEventLevel) (message.Level - 1);

				if(_logger.IsEnabled(level))
				{
					// TODO: Details
					// TODO: Terms

					var template = new MessageTemplate(new[] { new TextToken(message.Description.ToText()) });

					_logger.Write(new LogEvent(
						new DateTimeOffset(message.When.ToLocalTime()),
						level,
						message.Error ?? message.Details as Exception,
						template,
						Enumerable.Empty<LogEventProperty>()));
				}
			}
		}
	}
}