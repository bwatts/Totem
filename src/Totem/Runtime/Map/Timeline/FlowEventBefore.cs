using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .Before method observing an event in a <see cref="Flow"/>
	/// </summary>
	public sealed class FlowEventBefore
	{
		private readonly Lazy<Action<FlowEventContext>> _call;

		public FlowEventBefore(MethodInfo info, EventType eventType)
		{
			Info = info;
			EventType = eventType;

			_call = new Lazy<Action<FlowEventContext>>(CompileCall);
		}

		public readonly MethodInfo Info;
		public readonly EventType EventType;
		public Action<FlowEventContext> Call { get { return _call.Value; } }

		private Action<FlowEventContext> CompileCall()
		{
			// We are building an expression tree representing a call to this method:
			//
			// context => ((TFlow) context.Flow).Info((TEvent) context.Event)
			//
			// Let's break each part down, starting with the parameter:

			var context = Expression.Parameter(typeof(FlowEventContext), "context");

			// Cast the flow and event to their specific types:
			//
			// (TFlow) context.Flow
			// (TEvent) context.Event

			var flow = Expression.Convert(Expression.Field(context, "Flow"), Info.DeclaringType);
			var e = Expression.Convert(Expression.Field(context, "Event"), EventType.DeclaredType);

			// Call the method on the flow, passing the event and dependencies:
			//
			// flow.Info(e)

			var call = Expression.Call(flow, Info, e);

			// Create a lambda expression that makes the call:
			//
			// context => [call]

			var lambda = Expression.Lambda<Action<FlowEventContext>>(call, context);

			// Compile the lambda into a delegate we can invoke:

			return lambda.Compile();
		}
	}
}