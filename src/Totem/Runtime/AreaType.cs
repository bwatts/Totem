using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// A .NET type representing an area of related objects in a Totem runtime
	/// </summary>
	public sealed class AreaType : RuntimeType
	{
		public AreaType(RuntimePackage package, Type declaredType, string sectionName = "") : base(package, declaredType)
		{
			SectionName = sectionName;
			Dependencies = new AreaTypeSet();
		}

		public string SectionName { get; private set; }
		public AreaTypeSet Dependencies { get; private set; }
	}
}