using System;
using Totem.Core;

namespace Totem.Events
{
    public class EventContext<TEvent> : MessageContext, IEventContext<TEvent>
        where TEvent : class, IEvent
    {
        public EventContext(Id pipelineId, IEventEnvelope envelope) : base(pipelineId, envelope)
        {
            Envelope = envelope;

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
        public DateTimeOffset WhenOccurred => Envelope.WhenOccurred;
        public long TimelineVersion => Envelope.TimelineVersion;
    }
}