using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing a topic on the timeline
	/// </summary>
	public sealed class TopicType : FlowType
	{
		internal TopicType(RuntimeTypeRef type, FlowConstructor constructor, Many<RuntimeTypeKey> priorKeys)
      : base(type, constructor, priorKeys)
		{}

		public IEnumerable<FlowRoute> RouteGiven(Event e, bool scheduled = false)
		{
			var flowEvent = (TopicEvent) Events.Get(e);

			return flowEvent.RouteGiven(e, scheduled);
		}
	}
}