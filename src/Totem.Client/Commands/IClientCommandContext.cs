using System;
using System.Net.Http.Headers;
using Totem.Http;
using Totem.Core;

namespace Totem.Commands
{
    public interface IClientCommandContext<out TCommand> : IMessageContext
        where TCommand : ICommand
    {
        TCommand Command { get; }
        Type CommandType { get; }
        Id CommandId { get; }
        string ContentType { get; set; }
        HttpRequestHeaders Headers { get; }
        ClientResponse? Response { get; set; }
    }
}