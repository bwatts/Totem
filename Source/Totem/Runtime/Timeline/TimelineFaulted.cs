using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Indicates the timeline has an unhandled error
	/// </summary>
	public sealed class TimelineFaulted : Event
	{
		public TimelineFaulted(string fault)
		{
			Fault = fault;
		}

		public readonly string Fault;
	}
}