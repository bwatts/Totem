using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing a flow on the timeline
	/// </summary>
	public sealed class FlowType : RuntimeType
	{
		internal FlowType(RuntimeTypeRef type, bool isRequest, FlowConstructor constructor) : base(type)
		{
			IsRequest = isRequest;
			Constructor = constructor;
			Events = new FlowEventSet();
		}

		public readonly bool IsRequest;
		public readonly FlowConstructor Constructor;
		public readonly FlowEventSet Events;

		public Flow New()
		{
			return Constructor.Call();
		}

		public void CallBefore(Flow flow, Event e)
		{
			Events.CallBefore(flow, e);
		}

		public Task CallWhen(Flow flow, Event e, IDependencySource dependencies)
		{
			return Events.CallWhen(flow, e, dependencies);
		}
	}
}