using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .When method parameter representing a dependency resolved by key at call time
	/// </summary>
	public sealed class KeyedDependencyAttribute : DependencyAttribute
	{
		public KeyedDependencyAttribute(object key)
		{
			Key = key;
		}

		public readonly object Key;

		public override Dependency GetDependency(ParameterInfo parameter)
		{
			return Dependency.Keyed(parameter, Key);
		}
	}
}