using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A map of the elements within a Totem runtime
	/// </summary>
	public sealed class RuntimeMap
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

    public override string ToString() => Deployment.Folder.ToString();

		public DurableType GetDurable(RuntimeTypeKey key, bool strict = true)
		{
			return Regions.GetDurable(key, strict);
		}

		public DurableType GetDurable(Type declaredType, bool strict = true)
		{
			return Regions.GetDurable(declaredType, strict);
		}

		public AreaType GetArea(RuntimeTypeKey key, bool strict = true)
    {
      return Regions.GetArea(key, strict);
    }

    public AreaType GetArea(Type declaredType, bool strict = true)
    {
      return Regions.GetArea(declaredType, strict);
    }

    public EventType GetEvent(RuntimeTypeKey key, bool strict = true)
    {
      return Regions.GetEvent(key, strict);
    }

    public EventType GetEvent(Type declaredType, bool strict = true)
    {
      return Regions.GetEvent(declaredType, strict);
    }

    public FlowType GetFlow(RuntimeTypeKey key, bool strict = true)
    {
      return Regions.GetFlow(key, strict);
    }

    public FlowType GetFlow(Type declaredType, bool strict = true)
    {
      return Regions.GetFlow(declaredType, strict);
    }

    public TopicType GetTopic(RuntimeTypeKey key, bool strict = true)
    {
      return Regions.GetTopic(key, strict);
    }

    public TopicType GetTopic(Type declaredType, bool strict = true)
    {
      return Regions.GetTopic(declaredType, strict);
    }

    public ViewType GetView(RuntimeTypeKey key, bool strict = true)
    {
      return Regions.GetView(key, strict);
    }

    public ViewType GetView(Type declaredType, bool strict = true)
    {
      return Regions.GetView(declaredType, strict);
    }

    public RequestType GetRequest(RuntimeTypeKey key, bool strict = true)
    {
      return Regions.GetRequest(key, strict);
    }

    public RequestType GetRequest(Type declaredType, bool strict = true)
    {
      return Regions.GetRequest(declaredType, strict);
    }

    public WebApiType GetWebApi(RuntimeTypeKey key, bool strict = true)
    {
      return Regions.GetWebApi(key, strict);
    }

    public WebApiType GetWebApi(Type declaredType, bool strict = true)
    {
      return Regions.GetWebApi(declaredType, strict);
    }
  }
}