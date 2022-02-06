using System.Security.Claims;
using Totem.Core;

namespace Totem.Local;

public class LocalQueryEnvelope : MessageEnvelope, ILocalQueryEnvelope
{
    public LocalQueryEnvelope(ItemKey messageKey, ILocalQuery message, LocalQueryInfo info, Id correlationId, ClaimsPrincipal principal)
        : base(messageKey, message, info, correlationId, principal)
    { }

    public new ILocalQuery Message => (ILocalQuery) base.Message;
    public new LocalQueryInfo Info => (LocalQueryInfo) base.Info;

    ILocalMessage ILocalMessageEnvelope.Message => Message;
    ILocalMessageInfo ILocalMessageEnvelope.Info => Info;
    IQueryMessage IQueryEnvelope.Message => Message;
    QueryInfo IQueryEnvelope.Info => Info;
}
