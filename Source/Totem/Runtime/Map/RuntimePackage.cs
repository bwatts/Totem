using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Totem.IO;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of related aseemblies in a region of a Totem runtime
	/// </summary>
	public sealed class RuntimePackage
	{
		public RuntimePackage(string name, IFolder buildFolder, IFolder deploymentFolder, AssemblyCatalog catalog, RuntimeRegionKey regionKey)
		{
			Name = name;
			BuildFolder = buildFolder;
			DeploymentFolder = deploymentFolder;
			Catalog = catalog;
			RegionKey = regionKey;
			Assembly = catalog.Assembly;
			Areas = new RuntimeTypeSet<AreaType>();
      Events = new RuntimeTypeSet<EventType>();
			Flows = new RuntimeTypeSet<FlowType>();
      Topics = new RuntimeTypeSet<TopicType>();
      Queries = new RuntimeTypeSet<QueryType>();
      Views = new RuntimeTypeSet<ViewType>();
      Requests = new RuntimeTypeSet<RequestType>();
			WebApis = new RuntimeTypeSet<WebApiType>();
		}

		public readonly string Name;
		public readonly IFolder BuildFolder;
		public readonly IFolder DeploymentFolder;
		public readonly AssemblyCatalog Catalog;
		public readonly RuntimeRegionKey RegionKey;
		public readonly Assembly Assembly;
		public readonly RuntimeTypeSet<AreaType> Areas;
    public readonly RuntimeTypeSet<EventType> Events;
		public readonly RuntimeTypeSet<FlowType> Flows;
    public readonly RuntimeTypeSet<TopicType> Topics;
    public readonly RuntimeTypeSet<QueryType> Queries;
    public readonly RuntimeTypeSet<ViewType> Views;
    public readonly RuntimeTypeSet<RequestType> Requests;
		public readonly RuntimeTypeSet<WebApiType> WebApis;

    public override string ToString() => Name;
	}
}