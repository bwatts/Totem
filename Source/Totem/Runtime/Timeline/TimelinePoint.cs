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
		public TimelinePoint(TimelinePosition cause, TimelinePosition position, EventType eventType, Event e, bool scheduled)
		{
			Cause = cause;
			Position = position;
			EventType = eventType;
			Event = e;
			Scheduled = scheduled;
		}

		public readonly TimelinePosition Cause;
		public readonly TimelinePosition Position;
		public readonly EventType EventType;
		public readonly Event Event;
		public readonly bool Scheduled;

		public override string ToString() => $"{Position} {EventType}";
	}
}