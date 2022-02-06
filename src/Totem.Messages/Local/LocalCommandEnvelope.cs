using System.Security.Claims;
using Totem.Core;

namespace Totem.Local;

public class LocalCommandEnvelope : MessageEnvelope, ILocalCommandEnvelope
{
    public LocalCommandEnvelope(ItemKey messageKey, ILocalCommand message, LocalCommandInfo info, Id correlationId, ClaimsPrincipal principal)
        : base(messageKey, message, info, correlationId, principal)
    { }

    public new ILocalCommand Message => (ILocalCommand) base.Message;
    public new LocalCommandInfo Info => (LocalCommandInfo) base.Info;

    ILocalMessage ILocalMessageEnvelope.Message => Message;
    ILocalMessageInfo ILocalMessageEnvelope.Info => Info;

    ICommandMessage ICommandEnvelope.Message => Message;
    CommandInfo ICommandEnvelope.Info => Info;
}
