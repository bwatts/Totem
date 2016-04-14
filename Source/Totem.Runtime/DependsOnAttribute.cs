using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
	/// <summary>
	/// Indicates the decorated implementation of <see cref="IRuntimeArea"/> depends on another area type
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class DependsOnAttribute : Attribute
	{
		private readonly Type _areaType;
		private readonly RuntimeTypeKey _areaTypeKey;

		public DependsOnAttribute(Type areaType)
		{
			_areaType = areaType;
		}

		public DependsOnAttribute(string areaTypeKey)
		{
			_areaTypeKey = RuntimeTypeKey.From(areaTypeKey);
		}

		public AreaType GetArea(RuntimeMap map)
		{
			return _areaType != null
				? map.GetArea(_areaType)
				: map.GetArea(_areaTypeKey);
		}
	}
}