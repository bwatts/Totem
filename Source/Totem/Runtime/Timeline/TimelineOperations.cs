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
		public static void Append(this ITimeline timeline, TimelinePosition cause, Event e)
		{
			timeline.Append(cause, Many.Of(e));
		}

		public static void Append(this ITimeline timeline, TimelinePosition cause, IEnumerable<Event> events)
		{
			timeline.Append(cause, events.ToMany());
		}

		public static void Append(this ITimeline timeline, TimelinePosition cause, params Event[] events)
		{
			timeline.Append(cause, events.ToMany());
		}

		public static void Append(this ITimeline timeline, Event e)
		{
			timeline.Append(TimelinePosition.None, e);
		}

		public static void Append(this ITimeline timeline, IEnumerable<Event> events)
		{
			timeline.Append(TimelinePosition.None, events);
		}

		public static void Append(this ITimeline timeline, params Event[] events)
		{
			timeline.Append(TimelinePosition.None, events);
		}
	}
}