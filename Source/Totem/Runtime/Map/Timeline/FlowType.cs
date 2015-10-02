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
	public class FlowType : RuntimeType
	{
		internal FlowType(RuntimeTypeRef type, FlowConstructor constructor) : base(type)
		{
			Constructor = constructor;
			Events = new FlowEventSet();
			IsRequest = this is RequestFlowType;
		}

		public readonly FlowConstructor Constructor;
		public readonly FlowEventSet Events;
		public readonly bool IsRequest;

		public Flow New()
		{
			return Constructor.Call();
		}

		public bool CanCall(EventType e)
		{
			return Events.Contains(e);
		}

		public void CallBefore(Flow flow, TimelinePoint point)
		{
			Events.CallBefore(flow, point);
		}

		public Task CallWhen(Flow flow, TimelinePoint point, IDependencySource dependencies)
		{
			return Events.CallWhen(flow, point, dependencies);
		}
	}
}