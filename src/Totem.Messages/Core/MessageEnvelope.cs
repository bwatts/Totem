using System.Security.Claims;

namespace Totem.Core;

public abstract class MessageEnvelope : IMessageEnvelope
{
    protected MessageEnvelope(ItemKey messageKey, IMessage message, MessageInfo info, Id correlationId, ClaimsPrincipal principal)
    {
        MessageKey = messageKey ?? throw new ArgumentNullException(nameof(messageKey));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Info = info ?? throw new ArgumentNullException(nameof(info));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        Principal = principal ?? throw new ArgumentNullException(nameof(principal));
    }

    public ItemKey MessageKey { get; }
    public IMessage Message { get; }
    public MessageInfo Info { get; }
    public Id CorrelationId { get; }
    public ClaimsPrincipal Principal { get; }

    public override string ToString() =>
        MessageKey.ToString();
}
