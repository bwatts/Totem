using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A <see cref="TimelinePoint"/> routed to an instance of a flow
	/// </summary>
	public class FlowPoint : TimelinePoint
	{
		public FlowPoint(FlowRoute route, TimelinePoint point)
			: base(point.Position, point.Cause, point.EventType, point.Event, point.Scheduled)
		{
			Route = route;
		}

		public readonly FlowRoute Route;

		public override string ToString() => $"{base.ToString()} => {Route}";
	}
}