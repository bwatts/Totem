using System.Security.Claims;

namespace Totem.Core
{
    public class CommandEnvelope : MessageEnvelope, ICommandEnvelope
    {
        public CommandEnvelope(ICommand command, Id messageId, Id correlationId, ClaimsPrincipal principal)
            : base(command, messageId, correlationId, principal)
        {
            Message = command;
        }

        public new ICommand Message { get; }
    }
}