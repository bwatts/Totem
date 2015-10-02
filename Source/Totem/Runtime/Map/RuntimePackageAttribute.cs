using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// Indicates the decorated assembly defines a package of Totem runtime elements
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public sealed class RuntimePackageAttribute : Attribute
	{
		public RuntimePackageAttribute(string region)
		{
			Region = RuntimeRegionKey.From(region);
		}

		public RuntimeRegionKey Region { get; private set; }
	}
}