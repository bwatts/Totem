using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Extends <see cref="T:Totem.Runtime.Timeline.ITimeline"/>
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TimelineOperations
	{
		public static void Write(this ITimeline timeline, TimelinePosition cause, Event e)
		{
			timeline.Write(cause, Many.Of(e));
		}

		public static void Write(this ITimeline timeline, TimelinePosition cause, IEnumerable<Event> events)
		{
			timeline.Write(cause, events.ToMany());
		}

		public static void Write(this ITimeline timeline, TimelinePosition cause, params Event[] events)
		{
			timeline.Write(cause, events.ToMany());
		}

		public static void Write(this ITimeline timeline, Event e)
		{
			timeline.Write(TimelinePosition.None, e);
		}

		public static void Write(this ITimeline timeline, IEnumerable<Event> events)
		{
			timeline.Write(TimelinePosition.None, events);
		}

		public static void Write(this ITimeline timeline, params Event[] events)
		{
			timeline.Write(TimelinePosition.None, events);
		}
  }
}