using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing related functionality in a Totem runtime
	/// </summary>
	public sealed class AreaType : RuntimeType
	{
		internal AreaType(RuntimeTypeRef type, Many<IOResource> deployedResources) : base(type)
		{
			Dependencies = new RuntimeTypeSet<AreaType>();
			DeployedResources = deployedResources;
		}

		public readonly RuntimeTypeSet<AreaType> Dependencies;
		public readonly Many<IOResource> DeployedResources;
	}
}