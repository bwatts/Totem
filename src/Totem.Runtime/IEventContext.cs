using Totem.Core;
using Totem.Map;

namespace Totem;

public interface IEventContext<out TEvent> : IMessageContext
    where TEvent : IEvent
{
    new IEventEnvelope Envelope { get; }
    new EventInfo Info { get; }
    TEvent Event { get; }
    ItemKey EventKey { get; }
    EventType EventType { get; }
    Id EventId { get; }
    DateTimeOffset WhenOccurred { get; }
}
