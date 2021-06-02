using System;
using System.Security.Claims;

namespace Totem.Core
{
    public abstract class MessageEnvelope : IMessageEnvelope
    {
        protected MessageEnvelope(Id messageId, IMessage message, MessageInfo info, Id correlationId, ClaimsPrincipal principal)
        {
            MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Info = info ?? throw new ArgumentNullException(nameof(info));
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
            Principal = principal ?? throw new ArgumentNullException(nameof(principal));
        }

        public Id MessageId { get; }
        public IMessage Message { get; }
        public MessageInfo Info { get; }
        public Id CorrelationId { get; }
        public ClaimsPrincipal Principal { get; }

        public override string ToString() =>
            $"{Info.MessageType.Name}.{MessageId.ToShortString()}";
    }
}