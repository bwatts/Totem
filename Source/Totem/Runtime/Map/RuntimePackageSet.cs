using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of runtime regions, indexed by key
	/// </summary>
	public sealed class RuntimePackageSet : RuntimeSet<string, RuntimePackage>
	{
		public RuntimePackageSet(IEnumerable<RuntimePackage> packages)
		{
			foreach(var package in packages)
			{
				Register(package);
			}
		}

		internal override string GetKey(RuntimePackage value)
		{
			return value.Name;
		}

		public DurableType GetDurable(RuntimeTypeKey key, bool strict = true)
		{
			return GetPackageType(key, package => package.Durable, strict);
		}

		public DurableType GetDurable(Type declaredType, bool strict = true)
		{
			return GetPackageType(declaredType, package => package.Durable, strict);
		}

		public AreaType GetArea(RuntimeTypeKey key, bool strict = true)
    {
      return GetPackageType(key, package => package.Areas, strict);
    }

    public AreaType GetArea(Type declaredType, bool strict = true)
    {
      return GetPackageType(declaredType, package => package.Areas, strict);
    }

    public EventType GetEvent(RuntimeTypeKey key, bool strict = true)
    {
      return GetPackageType(key, package => package.Events, strict);
    }

    public EventType GetEvent(Type declaredType, bool strict = true)
    {
      return GetPackageType(declaredType, package => package.Events, strict);
    }

    public FlowType GetFlow(RuntimeTypeKey key, bool strict = true)
    {
      return GetPackageType(key, package => package.Flows, strict);
    }

    public FlowType GetFlow(Type declaredType, bool strict = true)
    {
      return GetPackageType(declaredType, package => package.Flows, strict);
    }

    public TopicType GetTopic(RuntimeTypeKey key, bool strict = true)
    {
      return GetPackageType(key, package => package.Topics, strict);
    }

    public TopicType GetTopic(Type declaredType, bool strict = true)
    {
      return GetPackageType(declaredType, package => package.Topics, strict);
    }

    public ViewType GetView(RuntimeTypeKey key, bool strict = true)
    {
      return GetPackageType(key, package => package.Views, strict);
    }

    public ViewType GetView(Type declaredType, bool strict = true)
    {
      return GetPackageType(declaredType, package => package.Views, strict);
    }

    public RequestType GetRequest(RuntimeTypeKey key, bool strict = true)
    {
      return GetPackageType(key, package => package.Requests, strict);
    }

    public RequestType GetRequest(Type declaredType, bool strict = true)
    {
      return GetPackageType(declaredType, package => package.Requests, strict);
    }

    public WebApiType GetWebApi(RuntimeTypeKey key, bool strict = true)
    {
      return GetPackageType(key, package => package.WebApis, strict);
    }

    public WebApiType GetWebApi(Type declaredType, bool strict = true)
    {
      return GetPackageType(declaredType, package => package.WebApis, strict);
    }

		private T GetPackageType<T>(RuntimeTypeKey key, Func<RuntimePackage, RuntimeTypeSetBase<T>> selectTypes, bool strict = true) where T : RuntimeType
    {
      var packageType = this
        .Select(package => selectTypes(package).Get(key, strict: false))
        .FirstOrDefault(type => type != null);

			ExpectNot(strict && packageType == null, $"Failed to resolve {typeof(T)} by key: {key}");

      return packageType;
    }

    private T GetPackageType<T>(Type declaredType, Func<RuntimePackage, RuntimeTypeSetBase<T>> selectTypes, bool strict = true) where T : RuntimeType
    {
      var packageType = this
        .Select(package => selectTypes(package).Get(declaredType, strict: false))
        .FirstOrDefault(type => type != null);

			ExpectNot(strict && packageType == null, $"Failed to resolve {typeof(T)} by declared type: {declaredType}");

      return packageType;
    }
  }
}