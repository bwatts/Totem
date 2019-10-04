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

    Task<FlowInfo> ReadFlow(FlowKey key);

    Task<FlowResumeInfo> ReadFlowToResume(FlowKey key);

    Task<TimelinePosition> WriteNewEvents(TimelinePosition cause, FlowKey topicKey, Many<Event> newEvents);

    Task WriteScheduledEvent(TimelinePoint cause);

    Task WriteCheckpoint(Flow flow, TimelinePoint point);
  }
}