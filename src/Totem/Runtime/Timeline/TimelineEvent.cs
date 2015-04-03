using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// New knowledge on the timeline of a distributed environment
	/// </summary>
	public abstract class TimelineEvent : Event
	{
		protected TimelineEvent(TimelinePosition sourcePosition)
		{
			SourcePosition = sourcePosition;
		}

		public readonly TimelinePosition SourcePosition;
		
		public abstract TimelineEventType Type { get; }
	}
}