using System.Collections.Specialized;
using System.Net;
using Totem.Core;
using Totem.Http;

namespace Totem;

public interface IHttpCommandContext<out TCommand> : ICommandContext<TCommand>
    where TCommand : IHttpCommand
{
    new IHttpCommandEnvelope Envelope { get; }
    new HttpCommandInfo Info { get; }
    HttpStatusCode ResponseCode { get; set; }
    NameValueCollection ResponseHeaders { get; }
    string? ResponseContentType { get; set; }
    object? ResponseContent { get; set; }
}
