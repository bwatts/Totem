using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes the series of events that occur in a domain
	/// </summary>
	public interface ITimeline : IFluent
	{
		void Append(TimelinePosition cause, IReadOnlyList<Event> events);

		TFlow Run<TFlow>(TimelinePosition cause, Event e) where TFlow : Flow;
	}
}