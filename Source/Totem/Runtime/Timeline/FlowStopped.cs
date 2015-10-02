using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Indicates a flow encountered an unhandled error and is not observing the timeline
	/// </summary>
	public sealed class FlowStopped : Event
	{
		public FlowStopped(RuntimeTypeKey type, string error)
		{
			Type = type;
			Error = error;
		}

		public readonly RuntimeTypeKey Type;
		public readonly string Error;
	}
}