using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of runtime regions, indexed by key
	/// </summary>
	public sealed class RuntimeRegionSet : RuntimeSetCore<RuntimeRegionKey, RuntimeRegion>
	{
		private readonly AssemblyRegionCache _assemblyRegions;

		public RuntimeRegionSet(IEnumerable<RuntimeRegion> regions)
		{
			foreach(var region in regions)
			{
				Register(region);
			}

			_assemblyRegions = new AssemblyRegionCache(this);
		}

		internal override RuntimeRegionKey GetKey(RuntimeRegion value)
		{
			return value.Key;
		}

		public RuntimePackage GetPackage(string name, bool strict = true)
		{
			var package = this
				.Select(region => region.GetPackage(name, strict: false))
				.FirstOrDefault(regionPackage => regionPackage != null);

			Expect(strict && package == null).IsFalse("Failed to resolve package: " + name);

			return package;
		}

		public AreaType GetArea(RuntimeTypeKey key, bool strict = true)
		{
			var region = Get(key.Region, strict);

			return region == null ? null : region.GetArea(key, strict);
		}

		public AreaType GetArea(Type declaredType, bool strict = true)
		{
			var region = _assemblyRegions.GetRegion(declaredType, strict);

			return region == null ? null : region.GetArea(declaredType, strict);
		}

		public ViewType GetView(RuntimeTypeKey key, bool strict = true)
		{
			var region = Get(key.Region, strict);

			return region == null ? null : region.GetView(key, strict);
		}

		public ViewType GetView(Type declaredType, bool strict = true)
		{
			var region = _assemblyRegions.GetRegion(declaredType, strict);

			return region == null ? null : region.GetView(declaredType, strict);
		}

		public FlowType GetFlow(RuntimeTypeKey key, bool strict = true)
		{
			var region = Get(key.Region, strict);

			return region == null ? null : region.GetFlow(key, strict);
		}

		public FlowType GetFlow(Type declaredType, bool strict = true)
		{
			var region = _assemblyRegions.GetRegion(declaredType, strict);

			return region == null ? null : region.GetFlow(declaredType, strict);
		}

		public EventType GetEvent(RuntimeTypeKey key, bool strict = true)
		{
			var region = Get(key.Region, strict);

			return region == null ? null : region.GetEvent(key, strict);
		}

		public EventType GetEvent(Type declaredType, bool strict = true)
		{
			var region = _assemblyRegions.GetRegion(declaredType, strict);

			return region == null ? null : region.GetEvent(declaredType, strict);
		}

		public WebApiType GetWebApi(RuntimeTypeKey key, bool strict = true)
		{
			var region = Get(key.Region, strict);

			return region == null ? null : region.GetWebApi(key, strict);
		}

		public WebApiType GetWebApi(Type declaredType, bool strict = true)
		{
			var region = _assemblyRegions.GetRegion(declaredType, strict);

			return region == null ? null : region.GetWebApi(declaredType, strict);
		}

		private sealed class AssemblyRegionCache
		{
			private readonly ConcurrentDictionary<Assembly, RuntimeRegionKey> _cache = new ConcurrentDictionary<Assembly, RuntimeRegionKey>();
			private readonly RuntimeRegionSet _regions;

			internal AssemblyRegionCache(RuntimeRegionSet regions)
			{
				_regions = regions;
			}

			internal RuntimeRegion GetRegion(Type declaredType, bool strict)
			{
				var key = _cache.GetOrAdd(declaredType.Assembly, _ => RuntimeRegionKey.From(declaredType.Assembly));

				return key == null ? null : _regions.Get(key, strict);
			}
		}
	}
}