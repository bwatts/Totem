using Totem.Core;

namespace Totem.Topics;

public interface ITopic : ITimeline
{
    bool HasNewEvents { get; }

    IReadOnlyList<IEvent> TakeNewEvents();
}
