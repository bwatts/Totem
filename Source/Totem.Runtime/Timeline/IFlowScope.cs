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
    FlowKey Key { get; }

    Task<Flow> Task { get; }

    FlowPoint Point { get; }

    void ResumeTo(TimelinePosition checkpoint);

    void Push(FlowPoint point);
  }
}