using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// One or more events will occur at a future point on the timeline
	/// </summary>
	public sealed class EventsScheduled : Event
	{
		public EventsScheduled(DateTime whenOccurs, IReadOnlyList<Event> events)
		{
			WhenOccurs = whenOccurs;
			Events = events;
		}

		public readonly DateTime WhenOccurs;
		public readonly IReadOnlyList<Event> Events;
	}
}