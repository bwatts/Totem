using System.Security.Claims;
using Totem.Core;

namespace Totem.Http
{
    public class HttpCommandEnvelope : MessageEnvelope, IHttpCommandEnvelope
    {
        public HttpCommandEnvelope(Id messageId, IHttpCommand message, HttpCommandInfo info, Id correlationId, ClaimsPrincipal principal)
            : base(messageId, message, info, correlationId, principal)
        { }

        public new IHttpCommand Message => (IHttpCommand) base.Message;
        public new HttpCommandInfo Info => (HttpCommandInfo) base.Info;
        IHttpMessage IHttpMessageEnvelope.Message => Message;
        HttpMessageInfo IHttpMessageEnvelope.Info => Info;
    }
}