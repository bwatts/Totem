using System;
using System.Security.Claims;

namespace Totem.Core;

public class EventEnvelope : MessageEnvelope, IEventEnvelope
{
    public EventEnvelope(
        ItemKey messageKey,
        IEvent message,
        EventInfo info,
        Id correlationId,
        ClaimsPrincipal principal,
        DateTimeOffset whenOccurred,
        TimelinePosition topicPosition)
        : base(messageKey, message, info, correlationId, principal)
    {
        WhenOccurred = whenOccurred;
        TopicPosition = topicPosition ?? throw new ArgumentNullException(nameof(topicPosition));
    }

    public new IEvent Message => (IEvent) base.Message;
    public new EventInfo Info => (EventInfo) base.Info;
    public TimelinePosition TopicPosition { get; }
    public DateTimeOffset WhenOccurred { get; }

    public override string ToString() =>
        $"{TopicPosition} => {MessageKey}";
}
