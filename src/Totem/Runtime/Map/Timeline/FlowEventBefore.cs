using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .Before method observing an event in a <see cref="Flow"/>
	/// </summary>
	public sealed class FlowEventBefore
	{
		private readonly Lazy<Action<Flow, Event>> _call;

		public FlowEventBefore(MethodInfo info, EventType eventType)
		{
			Info = info;
			EventType = eventType;

			_call = new Lazy<Action<Flow, Event>>(CompileCall);
		}

		public readonly MethodInfo Info;
		public readonly EventType EventType;
		public Action<Flow, Event> Call { get { return _call.Value; } }

		private Action<Flow, Event> CompileCall()
		{
			// We are building an expression tree representing a call to this method:
			//
			// (flow, e) => ((TFlow) flow).Info((TEvent) e)
			//
			// Let's break each part down, starting with the parameter:

			var flow = Expression.Parameter(typeof(Flow), "flow");
			var e = Expression.Parameter(typeof(Event), "e");

			// Cast the flow and event to their specific types:
			//
			// (TFlow) flow
			// (TEvent) e

			var castFlow = Expression.Convert(flow, Info.DeclaringType);
			var castEvent = Expression.Convert(e, EventType.DeclaredType);

			// Call the method on the flow, passing the event and dependencies:
			//
			// ((TFlow) flow).Info((TEvent) e)

			var call = Expression.Call(castFlow, Info, castEvent);

			// Create a lambda expression that makes the call:
			//
			// (flow, e) => [call]

			var lambda = Expression.Lambda<Action<Flow, Event>>(call, flow, e);

			// Compile the lambda into a delegate we can invoke:

			return lambda.Compile();
		}
	}
}