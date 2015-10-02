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
	/// A .When method observing an event in a <see cref="Flow"/>
	/// </summary>
	public sealed class FlowWhen : FlowMethod
	{
		private readonly Lazy<Func<Flow, Event, IDependencySource, Task>> _call;

		public FlowWhen(MethodInfo info, EventType eventType, Many<WhenDependency> dependencies) : base(info, eventType)
		{
			Dependencies = dependencies;

			IsAsync = typeof(Task).IsAssignableFrom(info.ReturnType);

			_call = new Lazy<Func<Flow, Event, IDependencySource, Task>>(CompileCall);
		}

		public readonly Many<WhenDependency> Dependencies;
		public readonly bool IsAsync;
		public Func<Flow, Event, IDependencySource, Task> Call { get { return _call.Value; } }

		private Func<Flow, Event, IDependencySource, Task> CompileCall()
		{
			// We are building an expression tree representing a call to this method:
			//
			// (flow, e, dependencies) => ((TFlow) flow).Info((TEvent) e, [resolved dependencies])
			//
			// Let's break each part down, starting with the parameter:

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

			if(!IsAsync)
			{
				// Flow methods participate in timeline work by wrapping work in tasks.
				//
				// Void methods require a parameterless lambda to run as tasks:
				//
				// () => [call]

				var runAction = Expression.Lambda<Action>(call);

				// Run a task that makes the call:
				//
				// Task.Run(() => [runAction])

				call = Expression.Call(_taskRunMethod, runAction);
			}

			// Create a lambda expression that makes the task-returning call:
			//
			// (flow, e, dependencies) => [call]

			var lambda = Expression.Lambda<Func<Flow, Event, IDependencySource, Task>>(call, flow, e, dependencies);

			// Compile the lambda into a delegate we can use to make the call:

			return lambda.Compile();
		}

		private static readonly MethodInfo _taskRunMethod =
		(
			from method in typeof(Task).GetMethods(BindingFlags.Public | BindingFlags.Static)
			where method.Name == "Run"
			let parameters = method.GetParameters()
			where parameters.Length == 1 && parameters[0].ParameterType == typeof(Action)
			select method
		)
		.First();
	}
}