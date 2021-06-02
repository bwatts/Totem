using System;
using Totem.Core;

namespace Totem
{
    public interface IEventContext<out TEvent> : IMessageContext
        where TEvent : IEvent
    {
        new IEventEnvelope Envelope { get; }
        TEvent Event { get; }
        Type EventType { get; }
        Id EventId { get; }
        Type TimelineType { get; }
        Id TimelineId { get; }
        long TimelineVersion { get; }
        DateTimeOffset WhenOccurred { get; }
    }
}