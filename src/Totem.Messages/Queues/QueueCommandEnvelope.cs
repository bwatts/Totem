using System.Security.Claims;
using Totem.Core;

namespace Totem.Queues;

public class QueueCommandEnvelope : MessageEnvelope, IQueueCommandEnvelope
{
    public QueueCommandEnvelope(ItemKey messageKey, IQueueCommand message, QueueCommandInfo info, Id correlationId, ClaimsPrincipal principal)
        : base(messageKey, message, info, correlationId, principal)
    { }

    public new IQueueCommand Message => (IQueueCommand) base.Message;
    public new QueueCommandInfo Info => (QueueCommandInfo) base.Info;

    ICommandMessage ICommandEnvelope.Message => Message;
    CommandInfo ICommandEnvelope.Info => Info;
}
