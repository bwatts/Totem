using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A point in the resuming of the timeline
	/// </summary>
	public class TimelineResumePointInfo
	{
		public TimelineResumePointInfo(TimelinePoint point, bool onSchedule)
		{
			Point = point;
			OnSchedule = onSchedule;
		}

		public readonly TimelinePoint Point;
		public readonly bool OnSchedule;
	}
}