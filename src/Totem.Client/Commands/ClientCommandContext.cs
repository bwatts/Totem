using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Totem.Http;
using Totem.Core;

namespace Totem.Commands
{
    public class ClientCommandContext<TCommand> : MessageContext, IClientCommandContext<TCommand>
        where TCommand : ICommand
    {
        public ClientCommandContext(Id pipelineId, ICommandEnvelope envelope) : base(pipelineId, envelope)
        {
            Envelope = envelope;

            if(envelope.Message is not TCommand command)
                throw new ArgumentException($"Expected command type {typeof(TCommand)} but received {envelope.MessageType}", nameof(envelope));

            Command = command;
        }

        public new ICommandEnvelope Envelope { get; }
        public TCommand Command { get; }
        public Type CommandType => Envelope.MessageType;
        public Id CommandId => Envelope.MessageId;
        public string ContentType { get; set; } = ContentTypes.Json;
        public HttpRequestHeaders Headers { get; } = new HttpRequestMessage().Headers;
        public ClientResponse? Response { get; set; }
    }
}