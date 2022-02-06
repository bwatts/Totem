using System.Security.Claims;
using Totem.Core;

namespace Totem.Http;

public class HttpQueryEnvelope : MessageEnvelope, IHttpQueryEnvelope
{
    public HttpQueryEnvelope(ItemKey messageKey, IHttpQuery message, HttpQueryInfo info, Id correlationId, ClaimsPrincipal principal)
        : base(messageKey, message, info, correlationId, principal)
    { }

    public new IHttpQuery Message => (IHttpQuery) base.Message;
    public new HttpQueryInfo Info => (HttpQueryInfo) base.Info;

    IHttpMessageInfo IHttpMessageEnvelope.Info => Info;
    IHttpMessage IHttpMessageEnvelope.Message => Message;
    IQueryMessage IQueryEnvelope.Message => Message;
    QueryInfo IQueryEnvelope.Info => Info;
}
