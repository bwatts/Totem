using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The information necessary to resume the timeline
	/// </summary>
	public class TimelineResumeInfo
	{
		public TimelineResumeInfo(Many<FlowType> flows, Many<TimelineResumePointInfo> points)
		{
			Flows = flows;
			Points = points;
		}

		public readonly Many<FlowType> Flows;
		public readonly Many<TimelineResumePointInfo> Points;
	}
}