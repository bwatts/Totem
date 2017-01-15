using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A method parameter representing a dependency resolved at call time
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public abstract class DependencyAttribute : Attribute
	{
		public abstract Dependency GetDependency(ParameterInfo parameter);
	}
}