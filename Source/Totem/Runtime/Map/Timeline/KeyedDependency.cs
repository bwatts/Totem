using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .When method parameter representing a dependency resolved by key
	/// </summary>
	public sealed class KeyedDependency : Dependency
	{
		public KeyedDependency(ParameterInfo parameter, object key) : base(parameter)
		{
			Key = key;
		}

		public readonly object Key;

		public override Expression ResolveIn(Expression dependencies)
		{
			// dependencies.ResolveKeyed<T>(Key)

			return Expression.Call(dependencies, "ResolveKeyed", new[] { Parameter.ParameterType }, Expression.Constant(Key));
		}
	}
}