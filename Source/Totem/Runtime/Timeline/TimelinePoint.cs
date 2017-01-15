using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// An event at a position on the timeline
	/// </summary>
	public class TimelinePoint
	{
		public TimelinePoint(
			TimelinePosition position,
			TimelinePosition cause,
			EventType eventType,
			Event e,
      bool scheduled = false)
		{
			Position = position;
			Cause = cause;
			EventType = eventType;
			Event = e;
			Scheduled = scheduled;

      RequestId = Flow.Traits.RequestId.Get(e);
      ClientId = Flow.Traits.ClientId.Get(e);
    }

		public readonly TimelinePosition Position;
		public readonly TimelinePosition Cause;
		public readonly EventType EventType;
		public readonly Event Event;
		public readonly bool Scheduled;
    public readonly Id RequestId;
    public readonly Id ClientId;

    public override string ToString() => $"{Position} {EventType}";
	}
}