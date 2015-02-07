using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Indicates the decorated implementation of <see cref="IRuntimeArea"/> depends on another area type
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class DependsOnAttribute : Attribute
	{
		public DependsOnAttribute(Type areaType)
		{
			AreaType = areaType;
		}

		public Type AreaType { get; private set; }
	}
}