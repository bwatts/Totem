using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A method defined by a flow and invoked as part of its lifecycle
	/// </summary>
	public sealed class FlowMethod
	{
		private readonly Lazy<Action<Flow, Event>> _call;

		internal FlowMethod(MethodInfo info, ParameterInfo eventParameter, EventType eventType)
		{
			Info = info;
			EventParameter = eventParameter;
			EventType = eventType;

			_call = new Lazy<Action<Flow, Event>>(BuildCall);
		}

		public readonly MethodInfo Info;
		public readonly ParameterInfo EventParameter;
		public readonly EventType EventType;
		public Action<Flow, Event> Call { get { return _call.Value; } }

		public override string ToString()
		{
			return Info.ToString();
		}

		private Action<Flow, Event> BuildCall()
		{
			// (flow, e) => ((TFlow) flow).Info((TEvent) e)

			var flow = Expression.Parameter(typeof(Flow), "flow");
			var e = Expression.Parameter(typeof(Event), "e");

			var convertFlow = Expression.Convert(flow, Info.DeclaringType);
			var convertEvent = Expression.Convert(e, EventParameter.ParameterType);

			var call = Expression.Call(convertFlow, Info, convertEvent);

			var lambda = Expression.Lambda<Action<Flow, Event>>(call, flow, e);

			return lambda.Compile();
		}
	}
}