using System.Security.Claims;

namespace Totem.Queues
{
    public static class QueueCommandEnvelopeExtensions
    {
        public static IQueueCommandEnvelope InEnvelope(this IQueueCommand command, Id correlationId, ClaimsPrincipal principal) =>
            new QueueCommandEnvelope(Id.NewId(), command, QueueCommandInfo.From(command), correlationId, principal);

        public static IQueueCommandEnvelope InEnvelope(this IQueueCommand command, Id correlationId) =>
            command.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

        public static IQueueCommandEnvelope InEnvelope(this IQueueCommand command, ClaimsPrincipal principal) =>
            command.InEnvelope(Id.NewId(), principal);

        public static IQueueCommandEnvelope InEnvelope(this IQueueCommand command) =>
            command.InEnvelope(Id.NewId());
    }
}