using System;
using System.Collections.Specialized;
using System.Net;
using Totem.Core;
using Totem.Map;

namespace Totem.Http;

public class HttpCommandContext<TCommand> : CommandContext<TCommand>, IHttpCommandContext<TCommand>
    where TCommand : IHttpCommand
{
    internal HttpCommandContext(Id pipelineId, IHttpCommandEnvelope envelope, CommandType commandType) : base(pipelineId, envelope, commandType)
    { }

    public new IHttpCommandEnvelope Envelope => (IHttpCommandEnvelope) base.Envelope;
    public new HttpCommandInfo Info => (HttpCommandInfo) base.Info;
    public override Type InterfaceType => typeof(IHttpCommandContext<TCommand>);
    public HttpStatusCode ResponseCode { get; set; } = HttpStatusCode.OK;
    public NameValueCollection ResponseHeaders { get; } = new(StringComparer.OrdinalIgnoreCase);
    public string? ResponseContentType { get; set; }
    public object? ResponseContent { get; set; }
}
