using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .When method parameter representing a dependency resolved by type
	/// </summary>
	public sealed class TypedDependency : Dependency
	{
		public TypedDependency(ParameterInfo parameter) : base(parameter)
		{}

		public override Expression ResolveIn(Expression dependencies)
		{
			// dependencies.Resolve<T>()

			return Expression.Call(dependencies, "Resolve", new[] { Parameter.ParameterType });
		}
	}
}