using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// An event from a source external to the timeline of a distributed environment
	/// </summary>
	public sealed class EventImported : TimelineEvent
	{
		public EventImported(Event e) : base(TimelinePosition.External)
		{
			Event = e;
		}

		public readonly Event Event;

		public override TimelineEventType Type
		{
			get { return TimelineEventType.Imported; }
		}

		public override Text ToText()
		{
			return Text.Of(SourcePosition).InBrackets() + " => " + Event;
		}
	}
}