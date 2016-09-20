using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// Makes decisions in reaction to an event within a <see cref="Flow"/>
	/// </summary>
	public sealed class WhenMethod : FlowMethod
	{
		private readonly Lazy<Action<Flow, Event, IDependencySource>> _call;
		private readonly Lazy<Func<Flow, Event, IDependencySource, Task>> _callAsync;

		internal WhenMethod(MethodInfo info, EventType eventType, Many<WhenDependency> dependencies) : base(info, eventType)
		{
			Dependencies = dependencies;

			if(typeof(Task).IsAssignableFrom(info.ReturnType))
			{
				IsAsync = true;

				_callAsync = CreateCall<Func<Flow, Event, IDependencySource, Task>>();
			}
			else
			{
				_call = CreateCall<Action<Flow, Event, IDependencySource>>();
			}
		}

		public readonly Many<WhenDependency> Dependencies;
		public readonly bool IsAsync;

		public async Task Call(Flow flow, Event e, IDependencySource dependencies)
		{
			if(IsAsync)
			{
				await _callAsync.Value(flow, e, dependencies);
			}
			else
			{
				_call.Value(flow, e, dependencies);
			}
		}

		private Lazy<TLambda> CreateCall<TLambda>() => new Lazy<TLambda>(CompileCall<TLambda>);

		private TLambda CompileCall<TLambda>()
		{
			// We are building an expression tree representing a call to this method:
			//
			// (flow, e, dependencies) => ((TFlow) flow).Info((TEvent) e, [resolved dependencies])
			//
			// Let's break each part down, starting with the parameters:

			var flow = Expression.Parameter(typeof(Flow), "flow");
			var e = Expression.Parameter(typeof(Event), "e");
			var dependencies = Expression.Parameter(typeof(IDependencySource), "e");

			// Cast the flow and event to their specific types:
			//
			// (TFlow) args.Flow
			// (TEvent) args.Event

			var castFlow = Expression.Convert(flow, Info.DeclaringType);
			var castEvent = Expression.Convert(e, EventType.DeclaredType);

			// Resolve dependencies:
			//
			// dependencies.Resolve<T>()
			// dependencies.ResolveNamed<T>("name")
			// dependencies.ResolveKeyed<T>(key)

			var resolvedDependencies = Dependencies.Select(dependency => dependency.ResolveIn(dependencies));

			// Call the method on the flow, passing the event and dependencies:
			//
			// ((TFlow) flow).Info((TEvent) e, [resolved dependencies])

			var call = Expression.Call(castFlow, Info, Many.Of(castEvent, resolvedDependencies));

			// Compile a lambda expression into a delegate we can invoke:
			//
			// (flow, e, dependencies) => [call]

			return Expression.Lambda<TLambda>(call, flow, e, dependencies).Compile();
		}
	}
}