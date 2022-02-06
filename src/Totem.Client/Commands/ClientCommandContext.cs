using System.Net.Http.Headers;
using Totem.Core;
using Totem.Http;

namespace Totem.Commands;

public class ClientCommandContext<TCommand> : MessageContext, IClientCommandContext<TCommand>
    where TCommand : IHttpCommand
{
    public ClientCommandContext(Id pipelineId, IHttpCommandEnvelope envelope) : base(pipelineId, envelope)
    {
        Envelope = envelope;

        if(envelope.Message is not TCommand command)
            throw new ArgumentException($"Expected command type {typeof(TCommand)} but received {envelope.MessageKey.DeclaredType}", nameof(envelope));

        Command = command;
    }

    public new IHttpCommandEnvelope Envelope { get; }
    public TCommand Command { get; }
    public Type CommandType => Envelope.MessageKey.DeclaredType;
    public Id CommandId => Envelope.MessageKey.Id;
    public string ContentType { get; set; } = ContentTypes.Json;
    public HttpRequestHeaders Headers { get; } = new HttpRequestMessage().Headers;
    public ClientResponse? Response { get; set; }
}
