using System;
using Totem.Events;
using Totem.Core;

namespace Totem.Routes
{
    public class RouteContext<TEvent> : MessageContext, IRouteContext<TEvent>
        where TEvent : IEvent
    {
        public RouteContext(Id pipelineId, IEventEnvelope envelope, IRouteAddress address) : base(pipelineId, envelope)
        {
            if(address == null)
                throw new ArgumentNullException(nameof(address));

            Envelope = envelope;
            RouteType = address.RouteType;
            RouteId = address.RouteId;

            if(envelope.Message is not TEvent e)
                throw new ArgumentException($"Expected event type {typeof(TEvent)} but received {envelope.Info.MessageType}", nameof(envelope));

            Event = e;
        }

        public new IEventEnvelope Envelope { get; }
        public TEvent Event { get; }
        public Type EventType => Envelope.Info.MessageType;
        public Id EventId => Envelope.MessageId;
        public Type TimelineType => Envelope.TimelineType;
        public Id TimelineId => Envelope.TimelineId;
        public long TimelineVersion => Envelope.TimelineVersion;
        public DateTimeOffset WhenOccurred => Envelope.WhenOccurred;
        public Type RouteType { get; }
        public Id RouteId { get; }
    }
}