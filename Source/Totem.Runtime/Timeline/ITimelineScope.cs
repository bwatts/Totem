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
    void Push(TimelinePoint point);

		Task<T> MakeRequest<T>(Id id) where T : Request;

    IFlowScope OpenFlowScope(FlowKey key);
  }
}