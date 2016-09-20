using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes the scope of a flow's activity on the timeline
	/// </summary>
	public interface IFlowScope : IConnectable
	{
    Flow Instance { get; }

    FlowKey Key { get; }

    FlowPoint Point { get; }

    Task Task { get; }

    void Push(FlowPoint point);
  }
}