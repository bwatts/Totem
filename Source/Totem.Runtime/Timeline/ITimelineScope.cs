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
		Task Push(TimelinePosition cause, Event e);

    Task Execute(Request request, Client client);
  }
}