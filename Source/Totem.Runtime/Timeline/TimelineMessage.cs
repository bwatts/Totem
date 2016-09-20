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
		public TimelineMessage(TimelinePoint point, Many<FlowRoute> routes = null)
		{
			Point = point;
			Routes = routes ?? new Many<FlowRoute>();
		}

		public readonly TimelinePoint Point;
		public readonly Many<FlowRoute> Routes;

		public override string ToString() => Point.ToString();
	}
}