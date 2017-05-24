using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Totem.Diagnostics;
using Totem.IO;
using Totem.Reflection;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Diagnostics;
using Totem.Runtime.Map.Timeline;
using Totem.Runtime.Timeline;

namespace Totem.Runtime
{
	/// <summary>
	/// Reads the regions of the runtime map and registers their elements
	/// </summary>
	internal sealed class RuntimeReader : Notion
	{
    readonly RuntimeDeployment _deployment;
		RuntimeMap _map;

    internal RuntimeReader(RuntimeDeployment deployment)
		{
			_deployment = deployment;
		}

		internal RuntimeMap Read()
		{
      _map = CreateMap();

			RegisterTypes();

			RegisterAreaDependencies();

			return _map;
		}

    RuntimeMap CreateMap()
    {
      return new RuntimeMap(_deployment, new RuntimeMonitor(), ReadRegions());
    }

		RuntimeRegionSet ReadRegions()
		{
			return new RuntimeRegionSet(
				from package in ReadPackages()
				group package by package.RegionKey into packagesByRegion
				select new RuntimeRegion(packagesByRegion.Key, new RuntimePackageSet(packagesByRegion)));
		}

		IEnumerable<RuntimePackage> ReadPackages()
		{
			var packages = Many.Of(ReadRuntimePackages(), ReadDeploymentPackages());

			if(_deployment.InSolution)
			{
				packages.Write.AddRange(ReadTotemSubmodulePackages());
			}

			return packages;
		}

		IEnumerable<RuntimePackage> ReadRuntimePackages()
		{
			yield return new RuntimePackage(
				"Totem",
				_deployment.HostFolder,
				_deployment.Folder,
				new AssemblyCatalog(typeof(RuntimeMap).Assembly),
				RuntimeRegionKey.From("runtime"));

			yield return new RuntimePackage(
				"Totem.Runtime",
				_deployment.HostFolder,
				_deployment.Folder,
				new AssemblyCatalog(typeof(RuntimeReader).Assembly),
				RuntimeRegionKey.From("runtime"));
		}

		IEnumerable<RuntimePackage> ReadDeploymentPackages()
		{
			return ReadPackages(_deployment.Folder, _deployment.Folder.ReadFolders());
		}

		IEnumerable<RuntimePackage> ReadTotemSubmodulePackages()
		{
			var submoduleFolder = _deployment.InTotemSubmodule
				? _deployment.Folder
				: _deployment.Folder.Up(1).Then(FolderResource.From("Submodules/Totem/Source"));

			var subfolders =
				from subfolder in submoduleFolder.ReadFolders()
				let path = subfolder.Path.ToString()
				where path != "Totem" && path != "Totem.Runtime"
				select subfolder;

			return ReadPackages(submoduleFolder, subfolders.ToMany());
		}

		IEnumerable<RuntimePackage> ReadPackages(IFolder folder, Many<FolderResource> subfolders)
		{
			return
				from subfolder in subfolders
				let name = subfolder.Path.ToString()
				let buildFolder = _deployment.ExpandBuild(subfolder)
				let primaryFile = ReadPackagePrimaryFile(folder, name, buildFolder)
				where primaryFile != null
				let catalog = new AssemblyCatalog(folder.Link.Then(primaryFile).ToString())
				let region = RuntimeRegionKey.From(catalog.Assembly, strict: false)
				where region != null
				select new RuntimePackage(
					name,
					folder.Then(buildFolder),
					folder.Then(subfolder),
					catalog,
					region);
		}

		static FileResource ReadPackagePrimaryFile(IFolder folder, string packageName, FolderResource buildFolder)
		{
			var dllFile = buildFolder.Then(FileName.From(packageName, "dll"));

			if(folder.FileExists(dllFile))
			{
				return dllFile;
			}

			var exeFile = buildFolder.Then(FileName.From(packageName, "exe"));

			if(folder.FileExists(exeFile))
			{
				return exeFile;
			}

			return null;
		}

    //
    // Types
    //

    RuntimePackage _package;
    Type _declaredType;

    void RegisterTypes()
		{
      foreach(var type in
				from region in _map.Regions
				from package in region.Packages
				from type in package.Assembly.GetTypes()
				where IsPublicOrInternal(type)
        where IsConcreteClassOrStatic(type)
        let isEvent = typeof(Event).IsAssignableFrom(type)
        orderby isEvent descending
				select new { package, type, isEvent })
			{
        _package = type.package;
        _declaredType = type.type;

				if(type.isEvent)
        {
          RegisterEvent();
        }
        else if(
          TryRegisterArea()
          || TryRegisterWebApi()
          || TryRegisterFlow()
          || TryRegisterDurable())
        {
          continue;
        }
        else
        {
          TryRegisterCounterCategory();
        }
			}

      _package = null;
      _declaredType = null;
    }

    static bool IsPublicOrInternal(Type type)
    {
      return type.IsPublic || type.IsNestedPublic || !type.IsNestedPrivate;
    }

    static bool IsConcreteClassOrStatic(Type type)
    {
      return type.IsClass && (!type.IsAbstract || type.IsSealed);
    }

    void RegisterEvent()
    {
			var e = new EventType(ReadType());

			_package.Durable.Register(e);
			_package.Events.Register(e);
    }

    bool TryRegisterArea()
    {
      if(DeclaredTypeIs<IRuntimeArea>())
      {
        var deployedResources =
          from attribute in _declaredType.GetCustomAttributes<DeployedResourceAttribute>(inherit: true)
          where attribute != null
          select attribute.Resource;

        _package.Areas.Register(new AreaType(ReadType(), deployedResources.ToMany()));

        return true;
      }

      return false;
    }

    bool TryRegisterWebApi()
    {
      if(DeclaredTypeIs<IWebApi>())
      {
        _package.WebApis.Register(new WebApiType(ReadType()));

        return true;
      }

      return false;
    }

    RuntimeTypeRef ReadType()
    {
      return new RuntimeTypeRef(_package, _declaredType);
    }

    bool DeclaredTypeIs<T>()
    {
      return typeof(T).IsAssignableFrom(_declaredType);
    }

    //
    // Flows
    //

    bool TryRegisterFlow()
    {
      if(DeclaredTypeIs<Flow>())
      {
        RegisterFlow();

        return true;
      }

      return false;
    }

    void RegisterFlow()
    {
      var type = ReadType();
      var constructor = ReadConstructor();
      var priorKeys = ReadPriorKeys().ToMany();

      if(DeclaredTypeIs<Topic>())
      {
        var topic = new TopicType(type, constructor, priorKeys);

        _package.Topics.Register(topic);

        RegisterFlow(topic);
      }
      else if(DeclaredTypeIs<View>())
      {
        var view = new ViewType(type, constructor, priorKeys, ReadBatchSize());

        _package.Views.Register(view);

        RegisterFlow(view);
      }
      else if(DeclaredTypeIs<Request>())
      {
        var start = new RuntimeReaderRequestStart(_map, _declaredType).Read();

        if(start != null)
        {
          var request = new RequestType(type, constructor, start);

          _package.Requests.Register(request);

          RegisterFlow(request);
        }
      }
      else
      {
        RegisterFlow(new FlowType(type, constructor, priorKeys));
      }
    }

    FlowConstructor ReadConstructor()
		{
			var constructors = _declaredType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

			switch(constructors.Length)
			{
				case 0:
					throw new Exception($"Flow {_declaredType} has no parameterless constructor");
				case 1:
					if(constructors[0].GetParameters().Any())
					{
						throw new Exception($"Flow {_declaredType} constructor has parameters: {constructors[0]}");
					}

					return new FlowConstructor(constructors[0]);
				default:
					throw new Exception($"Flow {_declaredType} has multiple constructors");
			}
		}

    IEnumerable<RuntimeTypeKey> ReadPriorKeys()
    {
      foreach(var attribute in _declaredType.GetCustomAttributes<PriorNameAttribute>(inherit: true))
      {
        var priorKey = RuntimeTypeKey.From(attribute.PriorName, strict: false);

        if(priorKey == null)
        {
          priorKey = RuntimeTypeKey.From(_package.RegionKey, attribute.PriorName);
        }

        yield return priorKey;
      }
    }

    int ReadBatchSize()
    {
      return _declaredType.GetCustomAttribute<BatchSizeAttribute>(inherit: true).BatchSize;
    }

    void RegisterFlow(FlowType flow)
    {
			_package.Durable.Register(flow);
			_package.Flows.Register(flow);

      new RuntimeReaderFlow(_map, flow).Register();
    }

		//
		// Durable
		//

		bool TryRegisterDurable()
		{
			if(IsDurable(_declaredType))
			{
				_package.Durable.Register(new DurableType(ReadType()));

        return true;
			}

      return false;
		}

		bool IsDurable(Type type)
		{
			var defined = _declaredType.IsDefined(typeof(DurableAttribute), inherit: true);

			var definedNested = !defined && type.IsNestedPublic && IsDurable(type.DeclaringType);

			return defined || definedNested;
		}

    //
    // Counters
    //

    void TryRegisterCounterCategory()
    {
      var declaration = _declaredType
        .GetCustomAttribute<CounterCategoryAttribute>()
        ?.GetCategory();

      if(declaration != null)
      {
        var category = new RuntimeCounterCategory(ReadType(), declaration);

        _map.Monitor.Categories.Register(category);

        var counters = _declaredType
          .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
          .Where(field => typeof(CounterBase).IsAssignableFrom(field.FieldType))
          .Select(field => (CounterBase) field.GetValue(null));

        category.Register(counters);
      }
    }

		//
		// Area dependencies
		//

		void RegisterAreaDependencies()
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