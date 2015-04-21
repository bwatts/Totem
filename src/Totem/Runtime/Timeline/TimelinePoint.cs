using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// An event at a position on the timeline
	/// </summary>
	public sealed class TimelinePoint
	{
		public TimelinePoint(TimelinePosition cause, TimelinePosition position, EventType eventType, Event e)
		{
			Cause = cause;
			Position = position;
			EventType = eventType;
			Event = e;
		}

		public readonly TimelinePosition Cause;
		public readonly TimelinePosition Position;
		public readonly EventType EventType;
		public readonly Event Event;

		public override string ToString()
		{
			return Text.Of("[{0}] {1}", Position, EventType);
		}
	}
}