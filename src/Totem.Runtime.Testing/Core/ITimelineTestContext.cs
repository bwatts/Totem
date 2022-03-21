using Totem.Map;

namespace Totem.Core;

public interface ITimelineTestContext<out TTimeline> where TTimeline : ITimeline
{
    RuntimeMap RuntimeMap { get; }
    TimelineType TimelineType { get; }
    TTimeline Timeline { get; }
}
