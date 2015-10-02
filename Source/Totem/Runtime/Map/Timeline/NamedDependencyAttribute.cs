using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .When method parameter representing a dependency resolved by name at call time
	/// </summary>
	public sealed class NamedDependencyAttribute : WhenDependencyAttribute
	{
		public NamedDependencyAttribute(string name)
		{
			Name = name;
		}

		public readonly string Name;

		public override WhenDependency GetDependency(ParameterInfo parameter)
		{
			return WhenDependency.Named(parameter, Name);
		}
	}
}