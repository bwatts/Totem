using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of runtime regions, indexed by key
	/// </summary>
	public sealed class RuntimePackageSet : RuntimeSetCore<string, RuntimePackage>
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
	}
}