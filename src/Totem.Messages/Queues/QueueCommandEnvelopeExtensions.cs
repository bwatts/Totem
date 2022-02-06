using System;
using System.Security.Claims;
using Totem.Core;

namespace Totem.Queues;

public static class QueueCommandEnvelopeExtensions
{
    public static IQueueCommandEnvelope InEnvelope(this IQueueCommand command, Id correlationId, ClaimsPrincipal principal)
    {
        if(command is null)
            throw new ArgumentNullException(nameof(command));

        var type = command.GetType();

        return new QueueCommandEnvelope(new ItemKey(type), command, QueueCommandInfo.From(type), correlationId, principal);
    }

    public static IQueueCommandEnvelope InEnvelope(this IQueueCommand command, Id correlationId) =>
        command.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

    public static IQueueCommandEnvelope InEnvelope(this IQueueCommand command, ClaimsPrincipal principal) =>
        command.InEnvelope(Id.NewId(), principal);

    public static IQueueCommandEnvelope InEnvelope(this IQueueCommand command) =>
        command.InEnvelope(Id.NewId());
}
