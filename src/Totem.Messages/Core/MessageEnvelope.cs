using System;
using System.Security.Claims;

namespace Totem.Core
{
    public abstract class MessageEnvelope : IMessageEnvelope
    {
        protected MessageEnvelope(IMessage message, Id messageId, Id correlationId, ClaimsPrincipal principal)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            MessageType = message.GetType();
            MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
            Principal = principal ?? throw new ArgumentNullException(nameof(principal));
        }

        public IMessage Message { get; }
        public Type MessageType { get; }
        public Id MessageId { get; }
        public Id CorrelationId { get; }
        public ClaimsPrincipal Principal { get; }

        public override string ToString() =>
            $"{MessageType.Name}.{MessageId.ToShortString()}";
    }
}