using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// The context of an event observed by a <see cref="Flow"/>
	/// </summary>
	public sealed class FlowEventContext
	{
		public FlowEventContext(Flow flow, Event e, EventType eventType, IDependencySource dependencies)
		{
			Flow = flow;
			Event = e;
			EventType = eventType;
			Dependencies = dependencies;
		}

		public readonly Flow Flow;
		public readonly Event Event;
		public readonly EventType EventType;
		public readonly IDependencySource Dependencies;

		public override string ToString()
		{
			return Text.Of("{0} => {1}", EventType, Flow);
		}
	}
}