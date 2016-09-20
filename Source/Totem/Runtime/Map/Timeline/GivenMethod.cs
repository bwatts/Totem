using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// Sets state in reaction to an event within a <see cref="Topic"/>
	/// </summary>
	public sealed class GivenMethod : FlowMethod
	{
		private readonly Lazy<Action<Topic, Event>> _call;

		internal GivenMethod(MethodInfo info, EventType eventType) : base(info, eventType)
		{
			_call = new Lazy<Action<Topic, Event>>(CompileCall);
		}

		public Action<Topic, Event> Call => _call.Value;

		private Action<Topic, Event> CompileCall()
		{
			// We are building an expression tree representing a call to this method:
			//
			// (topic, e) => ((TTopic) topic).Info((TEvent) e)
			//
			// Let's break each part down, starting with the parameter:

			var topic = Expression.Parameter(typeof(Topic), "topic");
			var e = Expression.Parameter(typeof(Event), "e");

			// Cast the topic and event to their specific types:
			//
			// (TTopic) topic
			// (TEvent) e

			var castTopic = Expression.Convert(topic, Info.DeclaringType);
			var castEvent = Expression.Convert(e, EventType.DeclaredType);

			// Call the method on the topic, passing the event and dependencies:
			//
			// ((TTopic) topic).Info((TEvent) e)

			var call = Expression.Call(castTopic, Info, castEvent);

			// Create a lambda expression that makes the call:
			//
			// (topic, e) => [call]

			var lambda = Expression.Lambda<Action<Topic, Event>>(call, topic, e);

			// Compile the lambda into a delegate we can invoke:

			return lambda.Compile();
		}
	}
}