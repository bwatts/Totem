using System;
using System.Security.Claims;
using Totem.Core;

namespace Totem.Local;

public static class LocalEnvelopeExtensions
{
    public static ILocalCommandEnvelope InEnvelope(this ILocalCommand command, Id correlationId, ClaimsPrincipal principal)
    {
        if(command is null)
            throw new ArgumentNullException(nameof(command));

        var type = command.GetType();

        return new LocalCommandEnvelope(new ItemKey(type), command, LocalCommandInfo.From(type), correlationId, principal);
    }

    public static ILocalCommandEnvelope InEnvelope(this ILocalCommand command, Id correlationId) =>
        command.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

    public static ILocalCommandEnvelope InEnvelope(this ILocalCommand command, ClaimsPrincipal principal) =>
        command.InEnvelope(Id.NewId(), principal);

    public static ILocalCommandEnvelope InEnvelope(this ILocalCommand command) =>
        command.InEnvelope(Id.NewId());

    public static ILocalQueryEnvelope InEnvelope(this ILocalQuery query, Id correlationId, ClaimsPrincipal principal)
    {
        if(query is null)
            throw new ArgumentNullException(nameof(query));

        var type = query.GetType();

        return new LocalQueryEnvelope(new ItemKey(type), query, LocalQueryInfo.From(type), correlationId, principal);
    }

    public static ILocalQueryEnvelope InEnvelope(this ILocalQuery query, Id correlationId) =>
        query.InEnvelope(correlationId, new ClaimsPrincipal(new ClaimsIdentity()));

    public static ILocalQueryEnvelope InEnvelope(this ILocalQuery query, ClaimsPrincipal principal) =>
        query.InEnvelope(Id.NewId(), principal);

    public static ILocalQueryEnvelope InEnvelope(this ILocalQuery query) =>
        query.InEnvelope(Id.NewId());
}
