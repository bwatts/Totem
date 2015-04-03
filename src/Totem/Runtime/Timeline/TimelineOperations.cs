using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Extends <see cref="ITimeline"/> to observe the various types of timeline events
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TimelineOperations
	{
		public static void Publish(this ITimeline timeline, TimelinePosition sourcePosition, string flowId, Event e)
		{
			timeline.Observe(new EventPublished(sourcePosition, flowId, e));
		}

		public static void Schedule(this ITimeline timeline, TimelinePosition sourcePosition, Event e, DateTime whenOccurs)
		{
			timeline.Observe(new EventScheduled(sourcePosition, e, whenOccurs));
		}

		public static void Import(this ITimeline timeline, Event e)
		{
			timeline.Observe(new EventImported(e));
		}

		public static void OnObserved(this ITimeline timeline, TimelinePosition sourcePosition, string flowId)
		{
			timeline.Observe(new EventObserved(sourcePosition, flowId));
		}
	}
}