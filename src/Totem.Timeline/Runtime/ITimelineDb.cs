using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// Describes a database containing an area's events and flows
  /// </summary>
  public interface ITimelineDb : IConnectable
  {
    Task<ResumeInfo> Subscribe(ITimelineObserver observer);

    Task<FlowResumeInfo> ReadFlowResumeInfo(FlowKey key);

    Task WriteScheduledEvent(TimelinePoint cause);

    Task<ImmediateGivens> WriteNewEvents(TimelinePosition cause, FlowKey topicKey, Many<Event> newEvents);

    Task WriteCheckpoint(Flow flow);
  }
}