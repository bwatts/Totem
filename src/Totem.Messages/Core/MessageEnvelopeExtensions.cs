using System.Security.Claims;

namespace Totem.Core
{
    public static class MessageEnvelopeExtensions
    {
        public static ICommandEnvelope InEnvelope(this ICommand command, Id correlationId, ClaimsPrincipal principal) =>
            new CommandEnvelope(command, Id.NewId(), correlationId, principal);

        public static ICommandEnvelope InEnvelope(this ICommand command, Id correlationId) =>
            command.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

        public static ICommandEnvelope InEnvelope(this ICommand command, ClaimsPrincipal principal) =>
            command.InEnvelope(Id.NewId(), principal);

        public static ICommandEnvelope InEnvelope(this ICommand command) =>
            command.InEnvelope(Id.NewId());

        public static IQueryEnvelope InEnvelope(this IQuery query, Id correlationId, ClaimsPrincipal principal) =>
            new QueryEnvelope(query, Id.NewId(), correlationId, principal);

        public static IQueryEnvelope InEnvelope(this IQuery query, Id correlationId) =>
            query.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

        public static IQueryEnvelope InEnvelope(this IQuery query, ClaimsPrincipal principal) =>
            query.InEnvelope(Id.NewId(), principal);

        public static IQueryEnvelope InEnvelope(this IQuery query) =>
            query.InEnvelope(Id.NewId());

        public static IQueueEnvelope InEnvelope(this IQueueCommand command, Id correlationId, ClaimsPrincipal principal, Text queueName) =>
            new QueueEnvelope(command, Id.NewId(), correlationId, principal, queueName);

        public static IQueueEnvelope InEnvelope(this IQueueCommand command, Id correlationId, ClaimsPrincipal principal) =>
            command.InEnvelope(correlationId, principal, QueueCommandInfo.From(command).QueueName);

        public static IQueueEnvelope InEnvelope(this IQueueCommand command, Id correlationId, Text queueName) =>
            command.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()), queueName);

        public static IQueueEnvelope InEnvelope(this IQueueCommand command, Id correlationId) =>
            command.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

        public static IQueueEnvelope InEnvelope(this IQueueCommand command, ClaimsPrincipal principal, Text queueName) =>
            command.InEnvelope(Id.NewId(), principal, queueName);

        public static IQueueEnvelope InEnvelope(this IQueueCommand command, ClaimsPrincipal principal) =>
            command.InEnvelope(Id.NewId(), principal);

        public static IQueueEnvelope InEnvelope(this IQueueCommand command, Text queueName) =>
            command.InEnvelope(Id.NewId(), queueName);

        public static IQueueEnvelope InEnvelope(this IQueueCommand command) =>
            command.InEnvelope(Id.NewId());
    }
}