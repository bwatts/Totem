using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes a database persisting timeline data
	/// </summary>
	public interface ITimelineDb
	{
		Many<TimelinePoint> Write(TimelinePosition cause, Many<Event> events);

		TimelinePoint WriteScheduled(TimelinePoint point);

		ResumeInfo ReadResumeInfo();
	}
}