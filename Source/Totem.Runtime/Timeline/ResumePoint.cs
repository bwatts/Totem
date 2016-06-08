using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A point in the resuming of the timeline
	/// </summary>
	public class ResumePoint
	{
		public ResumePoint(TimelinePoint point, bool onSchedule)
		{
			Point = point;
			OnSchedule = onSchedule;
		}

		public readonly TimelinePoint Point;
		public readonly bool OnSchedule;

    public override string ToString() => Point.ToString();
  }
}