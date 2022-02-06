using Totem.Core;
using Totem.Map;

namespace Totem.Events;

public class EventContext<TEvent> : MessageContext, IEventContext<TEvent>
    where TEvent : class, IEvent
{
    internal EventContext(Id pipelineId, IEventEnvelope envelope, EventType eventType) : base(pipelineId, envelope) =>
        EventType = eventType;

    public new IEventEnvelope Envelope => (IEventEnvelope) base.Envelope;
    public new EventInfo Info => Envelope.Info;
    public TEvent Event => (TEvent) Envelope.Message;
    public ItemKey EventKey => Envelope.MessageKey;
    public EventType EventType { get; }
    public Id EventId => Envelope.MessageKey.Id;
    public DateTimeOffset WhenOccurred => Envelope.WhenOccurred;
    public TimelinePosition TopicPosition => Envelope.TopicPosition;
}
