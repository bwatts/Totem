using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// Initiates the lifecycle of a <see cref="Flow"/>
	/// </summary>
	public sealed class FlowConstructor
	{
		private readonly Lazy<Func<Flow>> _call;

		public FlowConstructor(ConstructorInfo info)
		{
			Info = info;

			_call = new Lazy<Func<Flow>>(CompileCall);
		}

		public readonly ConstructorInfo Info;

    public Flow Call() => _call.Value();

		private Func<Flow> CompileCall()
		{
			// () => Info()

			var call = Expression.New(Info);

			var lambda = Expression.Lambda<Func<Flow>>(call);

			return lambda.Compile();
		}
	}
}