using System;
using System.Collections.Specialized;
using System.Net;
using Totem.Core;

namespace Totem.Http
{
    public class HttpCommandContext<TCommand> : MessageContext, IHttpCommandContext<TCommand>
        where TCommand : IHttpCommand
    {
        public HttpCommandContext(Id pipelineId, IHttpCommandEnvelope envelope) : base(pipelineId, envelope)
        {
            if(envelope.Message is not TCommand command)
                throw new ArgumentException($"Expected command type {typeof(TCommand)} but received {envelope.Info.MessageType}", nameof(envelope));

            Command = command;
        }

        public new IHttpCommandEnvelope Envelope => (IHttpCommandEnvelope) base.Envelope;
        public new HttpCommandInfo Info => (HttpCommandInfo) base.Info;
        public TCommand Command { get; }
        public Type CommandType => Envelope.Info.MessageType;
        public Id CommandId => Envelope.MessageId;
        public HttpStatusCode ResponseCode { get; set; } = HttpStatusCode.OK;
        public NameValueCollection ResponseHeaders { get; } = new(StringComparer.OrdinalIgnoreCase);
        public string? ResponseContentType { get; set; }
        public object? ResponseContent { get; set; }
    }
}