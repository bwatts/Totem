using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The types of events that occur on the domain timeline
	/// </summary>
	public enum TimelineEventType
	{
		/// <summary>
		/// A flow published an event to the timeline
		/// </summary>
		Published,

		/// <summary>
		/// A flow observed an event on the timeline
		/// </summary>
		Observed,

		/// <summary>
		/// An event will occur in the future
		/// </summary>
		Scheduled,

		/// <summary>
		/// An external event was represented on the timeline
		/// </summary>
		Imported
	}
}