using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Totem.Metrics;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A map of the elements within a Totem runtime
	/// </summary>
	public sealed class RuntimeMap
	{
    public RuntimeMap(RuntimeDeployment deployment, RuntimeRegionSet regions, RuntimeMonitor monitor)
    {
			Deployment = deployment;
      Regions = regions;
			Monitor = monitor;

			Catalog = new AggregateCatalog(
				from region in Regions
				from package in region.Packages
				select package.Catalog);
		}

		public readonly RuntimeDeployment Deployment;
		public readonly RuntimeRegionSet Regions;
    public readonly RuntimeMonitor Monitor;
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

    public RuntimeMetricType GetMetricType(RuntimeTypeKey key, bool strict = true)
    {
      return Monitor.GetMetricType(key, strict);
    }

    public RuntimeMetricType GetMetricType(Type declaredType, bool strict = true)
    {
      return Monitor.GetMetricType(declaredType, strict);
    }

    public RuntimeMetric GetMetric(RuntimeTypeKey key, bool strict = true)
    {
      return Monitor.GetMetric(key, strict);
    }

    public RuntimeMetric GetMetric(Metric declaration, bool strict = true)
    {
      return Monitor.GetMetric(declaration, strict);
    }
  }
}