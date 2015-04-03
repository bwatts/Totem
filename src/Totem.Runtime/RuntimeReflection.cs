using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Http;
using Totem.IO;
using Totem.Reflection;
using Totem.Runtime.Configuration;
using Totem.Runtime.Map;

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

		private sealed class MapReader
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
				var packages = ReadRuntimePackages();

				if(_deployment.InSolution)
				{
					packages = packages.Concat(ReadTotemPackages());
				}

				return packages;
			}

			private IEnumerable<RuntimePackage> ReadRuntimePackages()
			{
				return ReadPackages(_deployment.Folder);
			}

			private IEnumerable<RuntimePackage> ReadTotemPackages()
			{
				var submoduleFolder = _deployment.Folder.Up(1).Then(FolderResource.From("submodules/Totem/src"));

				return ReadPackages(submoduleFolder);
			}

			private IEnumerable<RuntimePackage> ReadPackages(IFolder folder)
			{
				return
					from subfolder in folder.ReadFolders(FolderResource.Root)
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
				var areaReads = new List<Action>();

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
					if(!TryReadView(type.package, type.declaredType))
					{
						areaReads.Add(() =>
						{
							if(!TryReadArea(type.package, type.declaredType))
							{
								TryReadApi(type.package, type.declaredType);
							}
						});
					}
				}

				foreach(var deferredRead in areaReads)
				{
					deferredRead();
				}
			}

			private static RuntimeTypeRef ReadType(RuntimePackage package, Type declaredType)
			{
				return new RuntimeTypeRef(package, declaredType, new RuntimeState(declaredType));
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
					var settingsViewType = typeof(RuntimeArea<>).GetAssignableGenericType(declaredType, strict: false);

					package.Areas.Register(new AreaType(
						ReadType(package, declaredType),
						settingsViewType == null ? null : package.GetView(settingsViewType)));

					return true;
				}

				return false;
			}

			//
			// APIs
			//

			private static void TryReadApi(RuntimePackage package, Type declaredType)
			{
				if(typeof(IWebApi).IsAssignableFrom(declaredType))
				{
					package.Apis.Register(ReadApi(package, declaredType));
				}
			}

			private static ApiType ReadApi(RuntimePackage package, Type declaredType)
			{
				var attribute = declaredType.GetCustomAttribute<WebApiAttribute>(inherit: true);

				return new ApiType(
					ReadType(package, declaredType),
					ReadResource(attribute),
					ReadResolve(declaredType, attribute));
			}

			private static HttpResource ReadResource(WebApiAttribute attribute)
			{
				return attribute != null ? attribute.Resource : HttpResource.Root;
			}

			private static Func<IDependencySource, IWebApi> ReadResolve(Type type, WebApiAttribute attribute)
			{
				return attribute == null || attribute.ResolveMethod == ""
					? ResolveByType(type)
					: ResolveByMethod(type, attribute.ResolveMethod);
			}

			private static Func<IDependencySource, IWebApi> ResolveByType(Type type)
			{
				return source => (IWebApi) source.Resolve(type);
			}

			private static Func<IDependencySource, IWebApi> ResolveByMethod(Type type, string methodName)
			{
				var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);

				Expect.That(method).IsNotNull("Missing resolve method: " + methodName);

				var parameters = method.GetParameters();

				Expect.That(parameters.Length != 1
					|| parameters[0].ParameterType != typeof(IDependencySource)
					|| method.ReturnType != typeof(IWebApi))
					.IsTrue("Invalid resolve method signature: " + Text.Of(method));

				var scopeParameter = Expression.Parameter(typeof(IDependencySource), "scope");

				var call = Expression.Call(method, scopeParameter);

				var callLambda = Expression.Lambda<Func<IDependencySource, IWebApi>>(call, scopeParameter);

				return callLambda.Compile();
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