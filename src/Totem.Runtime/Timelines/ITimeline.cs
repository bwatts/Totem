using System.Collections.Generic;
using Totem.Events;

namespace Totem.Timelines
{
    public interface ITimeline : IEventSourced
    {
        Id Id { get; }
        bool HasNewEvents { get; }
        IEnumerable<IEvent> NewEvents { get; }
    }
}