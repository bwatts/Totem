using System;
using System.Security.Claims;

namespace Totem.Core
{
    public interface IMessageEnvelope
    {
        IMessage Message { get; }
        Type MessageType { get; }
        Id MessageId { get; }
        Id CorrelationId { get; }
        ClaimsPrincipal Principal { get; }
    }
}