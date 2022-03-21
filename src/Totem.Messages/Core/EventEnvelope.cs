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
        DateTimeOffset whenOccurred)
        : base(messageKey, message, info, correlationId, principal)
    {
        WhenOccurred = whenOccurred;
    }

    public new IEvent Message => (IEvent) base.Message;
    public new EventInfo Info => (EventInfo) base.Info;
    public DateTimeOffset WhenOccurred { get; }

    public override string ToString() =>
        MessageKey.ToString();
}
