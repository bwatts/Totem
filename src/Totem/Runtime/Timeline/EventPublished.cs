using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A flow introduced new knowledge on the timeline of a distributed environment
	/// </summary>
	public sealed class EventPublished : TimelineEvent
	{
		public EventPublished(TimelinePosition sourcePosition, string flowId, Event e) : base(sourcePosition)
		{
			FlowId = flowId;
			Event = e;
		}

		public readonly string FlowId;
		public readonly Event Event;

		public override TimelineEventType Type
		{
			get { return TimelineEventType.Published; }
		}

		public override Text ToText()
		{
			return Text.Of(SourcePosition).InBrackets() + " " + FlowId + " => " + Event;
		}
	}
}