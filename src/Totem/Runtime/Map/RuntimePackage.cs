using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Totem.IO;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of related aseemblies in a region of a Totem runtime
	/// </summary>
	public sealed class RuntimePackage : Notion
	{
		public RuntimePackage(string name, IFolder folder, AssemblyCatalog catalog, RuntimeRegionKey regionKey)
		{
			Name = name;
			Folder = folder;
			Catalog = catalog;
			RegionKey = regionKey;
			Assembly = catalog.Assembly;
			Areas = new AreaTypeSet();
			Apis = new ApiTypeSet();
			Views = new ViewTypeSet();
		}

		public readonly string Name;
		public readonly IFolder Folder;
		public readonly AssemblyCatalog Catalog;
		public readonly RuntimeRegionKey RegionKey;
		public readonly Assembly Assembly;
		public readonly AreaTypeSet Areas;
		public readonly ApiTypeSet Apis;
		public readonly ViewTypeSet Views;

		public override Text ToText()
		{
			return Name;
		}

		public AreaType GetArea(RuntimeTypeKey key, bool strict = true)
		{
			return Areas.Get(key, strict);
		}

		public AreaType GetArea(Type declaredType, bool strict = true)
		{
			return Areas.Get(declaredType, strict);
		}

		public ApiType GetApi(RuntimeTypeKey key, bool strict = true)
		{
			return Apis.Get(key, strict);
		}

		public ApiType GetApi(Type declaredType, bool strict = true)
		{
			return Apis.Get(declaredType, strict);
		}

		public ViewType GetView(RuntimeTypeKey key, bool strict = true)
		{
			return Views.Get(key, strict);
		}

		public ViewType GetView(Type declaredType, bool strict = true)
		{
			return Views.Get(declaredType, strict);
		}
	}
}