using System.Collections.Generic;
using Totem.Core;

namespace Totem.Topics;

public interface ITopic : ITimeline
{
    bool HasNewEvents { get; }
    IEnumerable<IEvent> NewEvents { get; }
}
