using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .When method observing an event in a <see cref="Flow"/>
	/// </summary>
	public sealed class FlowEventWhen
	{
		private readonly Lazy<Func<FlowEventContext, Task>> _call;

		public FlowEventWhen(MethodInfo info, EventType eventType, IReadOnlyList<WhenDependency> dependencies)
		{
			Info = info;
			EventType = eventType;
			Dependencies = dependencies;

			IsFirst = info.Name == "WhenFirst";

			IsAsync = typeof(Task).IsAssignableFrom(info.ReturnType);

			_call = new Lazy<Func<FlowEventContext, Task>>(CompileCall);
		}

		public readonly MethodInfo Info;
		public readonly EventType EventType;
		public readonly IReadOnlyList<WhenDependency> Dependencies;
		public readonly bool IsFirst;
		public readonly bool IsAsync;
		public Func<FlowEventContext, Task> Call { get { return _call.Value; } }

		private Func<FlowEventContext, Task> CompileCall()
		{
			// We are building an expression tree representing a call to this method:
			//
			// context => ((TFlow) context.Flow).Info((TEvent) context.Event, [resolved dependencies])
			//
			// Let's break each part down, starting with the parameter:

			var context = Expression.Parameter(typeof(FlowEventContext), "context");

			// Cast the flow and event to their specific types:
			//
			// (TFlow) context.Flow
			// (TEvent) context.Event

			var flow = Expression.Convert(Expression.Field(context, "Flow"), Info.DeclaringType);
			var e = Expression.Convert(Expression.Field(context, "Event"), EventType.DeclaredType);

			// Resolve dependencies:
			//
			// context.Dependencies.Resolve<T>()
			// context.Dependencies.ResolveNamed<T>("name")
			// context.Dependencies.ResolveKeyed<T>(key)

			var dependencies = Expression.Field(context, "Dependencies");

			var resolvedDependencies = Dependencies.Select(dependency => dependency.ResolveIn(dependencies));

			// Call the method on the flow, passing the event and dependencies:
			//
			// [flow].Info([e], [resolved dependencies])

			var call = Expression.Call(flow, Info, Many.Of(e, resolvedDependencies));

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
			// context => [call]

			var lambda = Expression.Lambda<Func<FlowEventContext, Task>>(call, context);

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