using System.Security.Claims;
using Totem.Core;

namespace Totem.Http;

public class HttpCommandEnvelope : MessageEnvelope, IHttpCommandEnvelope
{
    public HttpCommandEnvelope(ItemKey messageKey, IHttpCommand message, HttpCommandInfo info, Id correlationId, ClaimsPrincipal principal)
        : base(messageKey, message, info, correlationId, principal)
    { }

    public new IHttpCommand Message => (IHttpCommand) base.Message;
    public new HttpCommandInfo Info => (HttpCommandInfo) base.Info;

    ICommandMessage ICommandEnvelope.Message => Message;
    CommandInfo ICommandEnvelope.Info => Info;

    IHttpMessage IHttpMessageEnvelope.Message => Message;
    IHttpMessageInfo IHttpMessageEnvelope.Info => Info;
}
