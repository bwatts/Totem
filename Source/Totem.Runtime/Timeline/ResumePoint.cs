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
		public ResumePoint(TimelineMessage message, bool onSchedule)
		{
			Message = message;
			OnSchedule = onSchedule;
		}

		public readonly TimelineMessage Message;
		public readonly bool OnSchedule;

    public override string ToString() => Message.Point.ToString();
  }
}