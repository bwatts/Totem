using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .When method parameter representing a dependency resolved by name
	/// </summary>
	public sealed class NamedDependency : WhenDependency
	{
		public NamedDependency(ParameterInfo parameter, string name) : base(parameter)
		{
			Name = name;
		}

		public readonly string Name;

		public override Expression ResolveIn(Expression dependencies)
		{
			// dependencies => dependencies.ResolveNamed<T>(Name)

			return Expression.Call(dependencies, "ResolveNamed", new[] { Parameter.ParameterType }, Expression.Constant(Name));
		}
	}
}