using System;
using Totem.Core;
using Totem.Events;

namespace Totem.Routes
{
    public interface IRouteContext<out TEvent> : IMessageContext
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
        Type RouteType { get; }
        Id RouteId { get; }
    }
}