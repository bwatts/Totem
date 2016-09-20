using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes the scope of timeline activity in a runtime
	/// </summary>
	public interface ITimelineScope : IConnectable
	{
		Task<T> MakeRequest<T>(TimelinePosition cause, Event e) where T : Request;

		void Push(TimelinePosition cause, Event e);
  }
}