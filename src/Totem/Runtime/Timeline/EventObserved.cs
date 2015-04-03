using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A flow observed an event on the timeline of a distributed environment
	/// </summary>
	public sealed class EventObserved : TimelineEvent
	{
		public EventObserved(TimelinePosition sourcePosition, string flowId) : base(sourcePosition)
		{
			FlowId = flowId;
		}

		public readonly string FlowId;

		public override TimelineEventType Type
		{
			get { return TimelineEventType.Observed; }
		}

		public override Text ToText()
		{
			return Text.Of(SourcePosition).InBrackets() + " => " + FlowId;
		}
	}
}