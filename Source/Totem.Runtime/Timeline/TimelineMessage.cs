using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A point on the timeline routed to flow instances
	/// </summary>
	public class TimelineMessage
	{
		public TimelineMessage(TimelinePoint point, Many<TimelineRoute> routes = null)
		{
			Point = point;
			Routes = routes ?? new Many<TimelineRoute>();
		}

		public readonly TimelinePoint Point;
		public readonly Many<TimelineRoute> Routes;

		public override string ToString() => Point.ToString();
	}
}