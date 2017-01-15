using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .When method parameter representing a dependency resolved by name at call time
	/// </summary>
	public sealed class NamedDependencyAttribute : DependencyAttribute
	{
		public NamedDependencyAttribute(string name)
		{
			Name = name;
		}

		public readonly string Name;

		public override Dependency GetDependency(ParameterInfo parameter)
		{
			return Dependency.Named(parameter, Name);
		}
	}
}