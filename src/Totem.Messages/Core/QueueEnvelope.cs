using System;
using System.Security.Claims;

namespace Totem.Core
{
    public class QueueEnvelope : MessageEnvelope, IQueueEnvelope
    {
        public QueueEnvelope(IQueueCommand command, Id messageId, Id correlationId, ClaimsPrincipal principal, Text queueName)
            : base(command, messageId, correlationId, principal)
        {
            Message = command;
            QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        public new IQueueCommand Message { get; }
        public Text QueueName { get; }
    }
}