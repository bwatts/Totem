using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A method parameter representing a dependency resolved at call time
	/// </summary>
	public abstract class Dependency
	{
		public Dependency(ParameterInfo parameter)
		{
			Parameter = parameter;
		}

		public ParameterInfo Parameter { get; private set; }

		public abstract Expression ResolveIn(Expression dependencies);

		//
		// Factory
		//

		public static TypedDependency Typed(ParameterInfo parameter)
		{
			return new TypedDependency(parameter);
		}

		public static NamedDependency Named(ParameterInfo parameter, string name)
		{
			return new NamedDependency(parameter, name);
		}

		public static KeyedDependency Keyed(ParameterInfo parameter, object key)
		{
			return new KeyedDependency(parameter, key);
		}
	}
}