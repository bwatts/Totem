using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of related areas in a Totem runtime
	/// </summary>
	public sealed class RuntimeRegion : Notion
	{
		public RuntimeRegion(RuntimeRegionKey key, RuntimePackageSet packages)
		{
			Key = key;
			Packages = packages;
		}

		public RuntimeRegionKey Key { get; private set; }
		public RuntimePackageSet Packages { get; private set; }

		public override Text ToText()
		{
			return Key.ToText();
		}

		public RuntimePackage GetPackage(string name, bool strict = true)
		{
			return Packages.Get(name, strict);
		}

		public DurableType GetDurable(RuntimeTypeKey key, bool strict = true)
		{
			return Packages.GetDurable(key, strict);
		}

		public DurableType GetDurable(Type declaredType, bool strict = true)
		{
			return Packages.GetDurable(declaredType, strict);
		}

		public AreaType GetArea(RuntimeTypeKey key, bool strict = true)
		{
      return Packages.GetArea(key, strict);
		}

		public AreaType GetArea(Type declaredType, bool strict = true)
		{
      return Packages.GetArea(declaredType, strict);
		}

    public EventType GetEvent(RuntimeTypeKey key, bool strict = true)
    {
      return Packages.GetEvent(key, strict);
    }

    public EventType GetEvent(Type declaredType, bool strict = true)
    {
      return Packages.GetEvent(declaredType, strict);
    }

    public FlowType GetFlow(RuntimeTypeKey key, bool strict = true)
    {
      return Packages.GetFlow(key, strict);
    }

    public FlowType GetFlow(Type declaredType, bool strict = true)
    {
      return Packages.GetFlow(declaredType, strict);
    }

    public TopicType GetTopic(RuntimeTypeKey key, bool strict = true)
    {
      return Packages.GetTopic(key, strict);
    }

    public TopicType GetTopic(Type declaredType, bool strict = true)
    {
      return Packages.GetTopic(declaredType, strict);
    }

    public QueryType GetQuery(RuntimeTypeKey key, bool strict = true)
    {
      return Packages.GetQuery(key, strict);
    }

    public QueryType GetQuery(Type declaredType, bool strict = true)
    {
      return Packages.GetQuery(declaredType, strict);
    }

    public ViewType GetView(RuntimeTypeKey key, bool strict = true)
    {
      return Packages.GetView(key, strict);
    }

    public ViewType GetView(Type declaredType, bool strict = true)
    {
      return Packages.GetView(declaredType, strict);
    }

    public RequestType GetRequest(RuntimeTypeKey key, bool strict = true)
    {
      return Packages.GetRequest(key, strict);
    }

    public RequestType GetRequest(Type declaredType, bool strict = true)
    {
      return Packages.GetRequest(declaredType, strict);
    }

		public WebApiType GetWebApi(RuntimeTypeKey key, bool strict = true)
		{
      return Packages.GetWebApi(key, strict);
    }

		public WebApiType GetWebApi(Type declaredType, bool strict = true)
		{
      return Packages.GetWebApi(declaredType, strict);
    }
	}
}