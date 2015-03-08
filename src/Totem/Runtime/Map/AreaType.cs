using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing an area of related functionality in a Totem runtime
	/// </summary>
	public sealed class AreaType : RuntimeType
	{
		public AreaType(RuntimeTypeRef type, string sectionName = "") : base(type)
		{
			SectionName = sectionName;
			Dependencies = new AreaTypeSet();
		}

		public string SectionName { get; private set; }
		public AreaTypeSet Dependencies { get; private set; }
	}
}