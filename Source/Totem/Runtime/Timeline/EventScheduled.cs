using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Indicates an event is scheduled to occur in the future
	/// </summary>
	public sealed class EventScheduled : Event
	{
		public EventScheduled(Event e)
		{
			Event = e;
		}

		public readonly Event Event;
	}
}