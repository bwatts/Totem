using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes the chronological context of a domain
	/// </summary>
	public interface ITimeline : IFluent
	{
		void Write(TimelinePosition cause, Many<Event> events);

		Task<T> MakeRequest<T>(Event e) where T : Request;
	}
}