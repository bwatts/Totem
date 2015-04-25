using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .When method parameter representing a dependency resolved by key at call time
	/// </summary>
	public sealed class KeyedDependencyAttribute : WhenDependencyAttribute
	{
		public KeyedDependencyAttribute(object key)
		{
			Key = key;
		}

		public readonly object Key;

		public override WhenDependency GetDependency(ParameterInfo parameter)
		{
			return WhenDependency.Keyed(parameter, Key);
		}
	}
}