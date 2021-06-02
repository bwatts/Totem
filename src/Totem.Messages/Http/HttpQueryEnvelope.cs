using System.Security.Claims;
using Totem.Core;

namespace Totem.Http
{
    public class HttpQueryEnvelope : MessageEnvelope, IHttpQueryEnvelope
    {
        public HttpQueryEnvelope(Id messageId, IHttpQuery message, HttpQueryInfo info, Id correlationId, ClaimsPrincipal principal)
            : base(messageId, message, info, correlationId, principal)
        { }

        public new IHttpQuery Message => (IHttpQuery) base.Message;
        public new HttpQueryInfo Info => (HttpQueryInfo) base.Info;
        IHttpMessage IHttpMessageEnvelope.Message => Message;
        HttpMessageInfo IHttpMessageEnvelope.Info => Info;
    }
}