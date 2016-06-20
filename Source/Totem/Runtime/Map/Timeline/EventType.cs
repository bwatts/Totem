using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing an event on the timeline
	/// </summary>
	public sealed class EventType : DurableType
	{
		public EventType(RuntimeTypeRef type) : base(type)
		{}

		public readonly Many<FlowEvent> Flows = new Many<FlowEvent>();

		public IEnumerable<TimelineRoute> CallRoute(Event e)
		{
			return
				from flowEvent in Flows
				where !flowEvent.FlowType.IsRequest
				from route in flowEvent.CallRoute(e)
				select route;
		}

		internal void RegisterFlow(FlowEvent e)
		{
			Flows.Write.Add(e);
		}
	}
}