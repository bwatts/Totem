using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes the series of events that occur in a domain
	/// </summary>
	public interface ITimeline : IFluent
	{
		void Append(TimelinePosition cause, IReadOnlyList<Event> events);

		Task<TFlow> MakeRequest<TFlow>(TimelinePosition cause, Event e) where TFlow : RequestFlow;
	}
}