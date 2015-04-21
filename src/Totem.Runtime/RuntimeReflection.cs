using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Totem.Http;
using Totem.IO;
using Totem.Reflection;
using Totem.Runtime.Configuration;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;
using Totem.Runtime.Timeline;

namespace Totem.Runtime
{
	/// <summary>
	/// Reflects deployed types to build a map of the runtime
	/// </summary>
	internal static class RuntimeReflection
	{
		internal static RuntimeMap ReadMap(this RuntimeSection section)
		{
			return new MapReader(section.ReadDeployment()).ReadMap();
		}

		private sealed class MapReader : Notion
		{
			private readonly RuntimeDeployment _deployment;
			private RuntimeMap _map;

			internal MapReader(RuntimeDeployment deployment)
			{
				_deployment = deployment;
			}

			internal RuntimeMap ReadMap()
			{
				_map = new RuntimeMap(_deployment, ReadRegions());

				RegisterTypes();

				RegisterDependencies();

				return _map;
			}

			//
			// Regions
			//

			private RuntimeRegionSet ReadRegions()
			{
				return new RuntimeRegionSet(
					from package in ReadPackages()
					group package by package.RegionKey into packagesByRegion
					select new RuntimeRegion(packagesByRegion.Key, new RuntimePackageSet(packagesByRegion)));
			}

			private IEnumerable<RuntimePackage> ReadPackages()
			{
				var packages = Many.Of(ReadRuntimePackage(), ReadDeploymentPackages());

				if(_deployment.InSolution)
				{
					packages.AddRange(ReadTotemSubmodulePackages());
				}

				return packages;
			}

			private RuntimePackage ReadRuntimePackage()
			{
				return new RuntimePackage(
					"Totem.Runtime",
					_deployment.HostFolder,
					new AssemblyCatalog(Assembly.GetExecutingAssembly()),
					RuntimeRegionKey.From("runtime"));
			}

			private IEnumerable<RuntimePackage> ReadDeploymentPackages()
			{
				return ReadPackages(_deployment.Folder, _deployment.Folder.ReadFolders(FolderResource.Root));
			}

			private IEnumerable<RuntimePackage> ReadTotemSubmodulePackages()
			{
				var submoduleFolder = _deployment.Folder.Up(1).Then(FolderResource.From("submodules/Totem/src"));

				var subfolders = submoduleFolder
					.ReadFolders(FolderResource.Root)
					.Where(subfolder => subfolder.Path.ToString() != "Totem.Runtime")
					.ToList();

				return ReadPackages(submoduleFolder, subfolders);
			}

			private IEnumerable<RuntimePackage> ReadPackages(IFolder folder, IReadOnlyList<FolderResource> subfolders)
			{
				return
					from subfolder in subfolders
					let name = subfolder.Path.ToString()
					let dllFile = ReadDllFile(subfolder, name)
					where folder.FileExists(dllFile)
					let catalog = new AssemblyCatalog(folder.Link.Then(dllFile).ToString())
					let region = RuntimeRegionKey.From(catalog.Assembly, strict: false)
					where region != null
					select new RuntimePackage(name, folder.Then(subfolder), catalog, region);
			}

			private FileResource ReadDllFile(FolderResource subfolder, string name)
			{
				if(_deployment.InSolution)
				{
					subfolder = subfolder.Then(FolderResource.From("bin/" + _deployment.SolutionConfiguration));
				}

				return subfolder.Then(FileName.From(name, "dll"));
			}

			//
			// Types
			//

			private void RegisterTypes()
			{
				var deferredReads = new List<Action>();

				foreach(var type in
					from region in _map.Regions
					from package in region.Packages
					from declaredType in package.Assembly.GetTypes()
					where declaredType.IsPublic
						&& declaredType.IsClass
						&& !declaredType.IsAbstract
						&& !declaredType.IsAnonymous()
					select new { package, declaredType })
				{
					if(!TryReadView(type.package, type.declaredType) && !TryReadEvent(type.package, type.declaredType))
					{
						deferredReads.Add(() =>
						{
							if(!TryReadArea(type.package, type.declaredType))
							{
								TryReadFlow(type.package, type.declaredType);
							}
						});
					}
				}

				foreach(var deferredRead in deferredReads)
				{
					deferredRead();
				}
			}

			private static RuntimeTypeRef ReadType(RuntimePackage package, Type declaredType)
			{
				return new RuntimeTypeRef(package, declaredType, new RuntimeState(declaredType));
			}

			private static bool TryReadEvent(RuntimePackage package, Type declaredType)
			{
				if(typeof(Event).IsAssignableFrom(declaredType))
				{
					package.Events.Register(new EventType(ReadType(package, declaredType)));

					return true;
				}

				return false;
			}

			private static bool TryReadView(RuntimePackage package, Type declaredType)
			{
				if(typeof(View).IsAssignableFrom(declaredType))
				{
					package.Views.Register(new ViewType(ReadType(package, declaredType)));

					return true;
				}

				return false;
			}

			private static bool TryReadArea(RuntimePackage package, Type declaredType)
			{
				if(typeof(IRuntimeArea).IsAssignableFrom(declaredType))
				{
					var typeWithSettings = typeof(RuntimeArea<>).GetAssignableGenericType(declaredType, strict: false);

					var settingsType = typeWithSettings == null ? null : typeWithSettings.GetGenericArguments().Single();

					package.Areas.Register(new AreaType(
						ReadType(package, declaredType),
						settingsType == null ? null : package.GetView(settingsType)));

					return true;
				}

				return false;
			}

			//
			// Flows
			//

			private void TryReadFlow(RuntimePackage package, Type declaredType)
			{
				if(typeof(Flow).IsAssignableFrom(declaredType))
				{
					package.Flows.Register(ReadFlow(package, declaredType));
				}
			}

			private FlowType ReadFlow(RuntimePackage package, Type declaredType)
			{
				var methodsByName = ReadFlowMethods(declaredType).ToLookup(method => method.Info.Name);

				return new FlowType(
					ReadType(package, declaredType),
					new FlowMethodSet(methodsByName["Before"].ToList()),
					new FlowMethodSet(methodsByName["When"].ToList()));
			}

			private IEnumerable<FlowMethod> ReadFlowMethods(Type declaredType)
			{
				foreach(var method in
					from sourceType in declaredType.GetInheritanceChainTo<Flow>(includeType: true, includeTargetType: true)
					from method in sourceType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					where method.IsPrivate
					select method)
				{
					FlowMethod flowMethod;

					if(TryReadFlowMethod(method, out flowMethod))
					{
						yield return flowMethod;
					}
				}
			}

			private bool TryReadFlowMethod(MethodInfo method, out FlowMethod flowMethod)
			{
				flowMethod = null;

				if(!TryReadBeforeOrWhenMethod(method, out flowMethod))
				{
					if(Text.EditDistance(method.Name, "Before") <= 3)
					{
						Log.Warning("[runtime] Flow method 'Before' possibly misspelled: {0}.{1}", method.DeclaringType.FullName, method.Name);
					}
					else
					{
						if(Text.EditDistance(method.Name, "When") <= 3)
						{
							Log.Warning("[runtime] Flow method 'When' possibly misspelled: {0}.{1}", method.DeclaringType.FullName, method.Name);
						}
					}
				}

				return flowMethod != null;
			}

			private bool TryReadBeforeOrWhenMethod(MethodInfo method, out FlowMethod flowMethod)
			{
				flowMethod = null;

				if(method.Name == "Before" || method.Name == "When")
				{
					var parameters = method.GetParameters();

					if(parameters.Length == 1 && typeof(Event).IsAssignableFrom(parameters[0].ParameterType))
					{
						flowMethod = new FlowMethod(method, parameters[0], _map.GetEvent(parameters[0].ParameterType));
					}
					else
					{
						Log.Warning("[runtime] Flow method {0}.{1} does not have the expected signature of a single event parameter", method.DeclaringType.FullName, method.Name);
					}
				}

				return flowMethod != null;
			}

			//
			// Dependencies
			//

			private void RegisterDependencies()
			{
				foreach(var dependency in
					from region in _map.Regions
					from package in region.Packages
					from area in package.Areas
					from dependsOnAttribute in area.DeclaredType.GetCustomAttributes<DependsOnAttribute>(inherit: true)
					select new
					{
						Source = area,
						Target = dependsOnAttribute.GetArea(_map)
					})
				{
					dependency.Source.Dependencies.Register(dependency.Target);
				}
			}
		}
	}
}