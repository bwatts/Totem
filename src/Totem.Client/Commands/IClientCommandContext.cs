using System.Net.Http.Headers;
using Totem.Core;
using Totem.Http;

namespace Totem.Commands;

public interface IClientCommandContext<out TCommand> : IMessageContext
    where TCommand : IHttpCommand
{
    TCommand Command { get; }
    Type CommandType { get; }
    Id CommandId { get; }
    string ContentType { get; set; }
    HttpRequestHeaders Headers { get; }
    ClientResponse? Response { get; set; }
}
