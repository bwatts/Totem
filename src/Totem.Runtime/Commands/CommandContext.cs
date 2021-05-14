using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using Totem.Core;

namespace Totem.Commands
{
    public class CommandContext<TCommand> : MessageContext, ICommandContext<TCommand>
        where TCommand : ICommand
    {
        public CommandContext(Id pipelineId, ICommandEnvelope envelope) : base(pipelineId, envelope)
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
        public ClaimsPrincipal? User { get; set; }
        public HttpStatusCode ResponseCode { get; set; } = HttpStatusCode.OK;
        public NameValueCollection ResponseHeaders { get; } = new(StringComparer.OrdinalIgnoreCase);
        public string? ResponseContentType { get; set; }
        public object? ResponseContent { get; set; }
    }
}