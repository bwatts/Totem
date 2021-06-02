using System.Security.Claims;

namespace Totem.Core
{
    public interface IMessageEnvelope
    {
        Id MessageId { get; }
        IMessage Message { get; }
        MessageInfo Info { get; }
        Id CorrelationId { get; }
        ClaimsPrincipal Principal { get; }
    }
}