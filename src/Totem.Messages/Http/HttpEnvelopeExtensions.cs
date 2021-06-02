using System.Security.Claims;

namespace Totem.Http
{
    public static class HttpEnvelopeExtensions
    {
        public static IHttpCommandEnvelope InEnvelope(this IHttpCommand command, Id correlationId, ClaimsPrincipal principal) =>
            new HttpCommandEnvelope(Id.NewId(), command, HttpCommandInfo.From(command), correlationId, principal);

        public static IHttpCommandEnvelope InEnvelope(this IHttpCommand command, Id correlationId) =>
            command.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

        public static IHttpCommandEnvelope InEnvelope(this IHttpCommand command, ClaimsPrincipal principal) =>
            command.InEnvelope(Id.NewId(), principal);

        public static IHttpCommandEnvelope InEnvelope(this IHttpCommand command) =>
            command.InEnvelope(Id.NewId());

        public static IHttpQueryEnvelope InEnvelope(this IHttpQuery query, Id correlationId, ClaimsPrincipal principal) =>
            new HttpQueryEnvelope(Id.NewId(), query, HttpQueryInfo.From(query) , correlationId, principal);

        public static IHttpQueryEnvelope InEnvelope(this IHttpQuery query, Id correlationId) =>
            query.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

        public static IHttpQueryEnvelope InEnvelope(this IHttpQuery query, ClaimsPrincipal principal) =>
            query.InEnvelope(Id.NewId(), principal);

        public static IHttpQueryEnvelope InEnvelope(this IHttpQuery query) =>
            query.InEnvelope(Id.NewId());
    }
}