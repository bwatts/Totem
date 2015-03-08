using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of runtime regions, indexed by key
	/// </summary>
	public sealed class RuntimeRegionSet : RuntimeSetCore<RuntimeRegionKey, RuntimeRegion>
	{
		public RuntimeRegionSet(IEnumerable<RuntimeRegion> regions)
		{
			foreach(var region in regions)
			{
				Register(region);
			}
		}

		internal override RuntimeRegionKey GetKey(RuntimeRegion value)
		{
			return value.Key;
		}
	}
}