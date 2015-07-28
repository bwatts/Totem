using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes the hosting of a flow on the timeline
	/// </summary>
	public interface IFlowHost : ITimelineScope
	{
		FlowType Type { get; }

		Task<Flow> Task { get; }
	}
}