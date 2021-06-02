using System.Security.Claims;

namespace Totem.Local
{
    public static class LocalEnvelopeExtensions
    {
        public static ILocalCommandEnvelope InEnvelope(this ILocalCommand command, Id correlationId, ClaimsPrincipal principal) =>
            new LocalCommandEnvelope(Id.NewId(), command, LocalCommandInfo.From(command), correlationId, principal);

        public static ILocalCommandEnvelope InEnvelope(this ILocalCommand command, Id correlationId) =>
            command.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

        public static ILocalCommandEnvelope InEnvelope(this ILocalCommand command, ClaimsPrincipal principal) =>
            command.InEnvelope(Id.NewId(), principal);

        public static ILocalCommandEnvelope InEnvelope(this ILocalCommand command) =>
            command.InEnvelope(Id.NewId());

        public static ILocalQueryEnvelope InEnvelope(this ILocalQuery query, Id correlationId, ClaimsPrincipal principal) =>
            new LocalQueryEnvelope(Id.NewId(), query, LocalQueryInfo.From(query) , correlationId, principal);

        public static ILocalQueryEnvelope InEnvelope(this ILocalQuery query, Id correlationId) =>
            query.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

        public static ILocalQueryEnvelope InEnvelope(this ILocalQuery query, ClaimsPrincipal principal) =>
            query.InEnvelope(Id.NewId(), principal);

        public static ILocalQueryEnvelope InEnvelope(this ILocalQuery query) =>
            query.InEnvelope(Id.NewId());
    }
}