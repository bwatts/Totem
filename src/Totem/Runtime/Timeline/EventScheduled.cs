using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// An event will occur in the future on the timeline of a distributed environment
	/// </summary>
	public sealed class EventScheduled : TimelineEvent
	{
		public EventScheduled(TimelinePosition sourcePosition, Event e, DateTime whenOccurs) : base(sourcePosition)
		{
			Event = e;
			WhenOccurs = whenOccurs;
		}

		public readonly Event Event;
		public readonly DateTime WhenOccurs;

		public override TimelineEventType Type
		{
			get { return TimelineEventType.Scheduled; }
		}

		public override Text ToText()
		{
			return Text.Of(SourcePosition).InBrackets() + " => " + Event + " @ " + WhenOccurs;
		}
	}
}