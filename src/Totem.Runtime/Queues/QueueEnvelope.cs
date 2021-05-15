using System;
using System.Security.Claims;
using Totem.Core;

namespace Totem.Queues
{
    public class QueueEnvelope : MessageEnvelope, IQueueEnvelope
    {
        public QueueEnvelope(IQueueCommand command, Id commandId, Id correlationId, ClaimsPrincipal principal, Text queueName)
            : base(command, commandId, correlationId, principal)
        {
            Message = command;
            QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        public new IQueueCommand Message { get; }
        public Text QueueName { get; }

        public override string ToString() =>
            $"{MessageType.Name}.{MessageId.ToShortString()} => {QueueName}";
    }
}