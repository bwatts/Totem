namespace Totem.Http.Commands;

public class HttpCommandClientContext<TCommand> : MessageContext, IHttpCommandClientContext<TCommand>
    where TCommand : IHttpCommand
{
    public HttpCommandClientContext(Id pipelineId, IHttpCommandEnvelope envelope) : base(pipelineId, envelope)
    { }

    public new IHttpCommandEnvelope Envelope => (IHttpCommandEnvelope) base.Envelope;
    public new HttpCommandInfo Info => (HttpCommandInfo) base.Info;
    public TCommand Command => (TCommand) Envelope.Message;
    public ItemKey CommandKey => Envelope.MessageKey;
    public Type CommandType => Envelope.MessageKey.DeclaredType;
    public Id CommandId => Envelope.MessageKey.Id;
    public string ContentType { get; set; } = ContentTypes.Json;
    public HttpRequestHeaders Headers { get; } = new HttpRequestMessage().Headers;
    public ClientResponse? Response { get; set; }
}
