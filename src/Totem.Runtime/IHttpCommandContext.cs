using System;
using System.Collections.Specialized;
using System.Net;
using Totem.Core;
using Totem.Http;

namespace Totem
{
    public interface IHttpCommandContext<out TCommand> : IMessageContext
        where TCommand : IHttpCommand
    {
        new IHttpCommandEnvelope Envelope { get; }
        new HttpCommandInfo Info { get; }
        TCommand Command { get; }
        Type CommandType { get; }
        Id CommandId { get; }
        HttpStatusCode ResponseCode { get; set; }
        NameValueCollection ResponseHeaders { get; }
        string? ResponseContentType { get; set; }
        object? ResponseContent { get; set; }
    }
}