using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// Describes the scope of a flow's activity on the timeline
  /// </summary>
  public interface IFlowScope : IConnectable
  {
    FlowKey Key { get; }

    Task<FlowResult> Task { get; }

    void Enqueue(TimelinePoint point);
  }
}