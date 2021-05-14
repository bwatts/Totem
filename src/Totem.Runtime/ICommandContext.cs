using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using Totem.Core;

namespace Totem
{
    public interface ICommandContext<out TCommand> : IMessageContext
        where TCommand : ICommand
    {
        new ICommandEnvelope Envelope { get; }
        TCommand Command { get; }
        Type CommandType { get; }
        Id CommandId { get; }
        ClaimsPrincipal? User { get; set; }
        HttpStatusCode ResponseCode { get; set; }
        NameValueCollection ResponseHeaders { get; }
        string? ResponseContentType { get; set; }
        object? ResponseContent { get; set; }
    }
}