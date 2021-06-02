using System.Security.Claims;
using Totem.Core;

namespace Totem.Local
{
    public class LocalCommandEnvelope : MessageEnvelope, ILocalCommandEnvelope
    {
        public LocalCommandEnvelope(Id messageId, ILocalCommand message, LocalCommandInfo info, Id correlationId, ClaimsPrincipal principal)
            : base(messageId, message, info, correlationId, principal)
        { }

        public new ILocalCommand Message => (ILocalCommand) base.Message;
        public new LocalCommandInfo Info => (LocalCommandInfo) base.Info;
        ILocalMessage ILocalMessageEnvelope.Message => Message;
        LocalMessageInfo ILocalMessageEnvelope.Info => Info;
    }
}