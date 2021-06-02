using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Totem.Http;
using Totem.Core;

namespace Totem.Commands
{
    public class ClientCommandContext<TCommand> : MessageContext, IClientCommandContext<TCommand>
        where TCommand : IHttpCommand
    {
        public ClientCommandContext(Id pipelineId, IHttpCommandEnvelope envelope) : base(pipelineId, envelope)
        {
            Envelope = envelope;

            if(envelope.Message is not TCommand command)
                throw new ArgumentException($"Expected command type {typeof(TCommand)} but received {envelope.Info.MessageType}", nameof(envelope));

            Command = command;
        }

        public new IHttpCommandEnvelope Envelope { get; }
        public TCommand Command { get; }
        public Type CommandType => Envelope.Info.MessageType;
        public Id CommandId => Envelope.MessageId;
        public string ContentType { get; set; } = ContentTypes.Json;
        public HttpRequestHeaders Headers { get; } = new HttpRequestMessage().Headers;
        public ClientResponse? Response { get; set; }
    }
}