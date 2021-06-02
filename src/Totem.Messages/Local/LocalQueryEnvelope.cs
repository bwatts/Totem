using System.Security.Claims;
using Totem.Core;

namespace Totem.Local
{
    public class LocalQueryEnvelope : MessageEnvelope, ILocalQueryEnvelope
    {
        public LocalQueryEnvelope(Id messageId, ILocalQuery message, LocalQueryInfo info, Id correlationId, ClaimsPrincipal principal)
            : base(messageId, message, info, correlationId, principal)
        { }

        public new ILocalQuery Message => (ILocalQuery) base.Message;
        public new LocalQueryInfo Info => (LocalQueryInfo) base.Info;
        ILocalMessage ILocalMessageEnvelope.Message => Message;
        LocalMessageInfo ILocalMessageEnvelope.Info => Info;
    }
}