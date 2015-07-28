using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes the database persisting the timeline
	/// </summary>
	public interface ITimelineDb
	{
		Many<TimelinePoint> Append(TimelinePosition cause, Many<Event> events);

		TimelinePoint AppendOccurred(TimelinePoint scheduledPoint);

		void RemoveFromSchedule(TimelinePosition position);

		TimelineResumeInfo ReadResumeInfo();
	}
}