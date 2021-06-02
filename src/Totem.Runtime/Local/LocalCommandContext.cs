using System;
using Totem.Core;

namespace Totem.Local
{
    public class LocalCommandContext<TCommand> : MessageContext, ILocalCommandContext<TCommand>
        where TCommand : ILocalCommand
    {
        public LocalCommandContext(Id pipelineId, ILocalCommandEnvelope envelope) : base(pipelineId, envelope)
        {
            if(envelope.Message is not TCommand command)
                throw new ArgumentException($"Expected command type {typeof(TCommand)} but received {envelope.Info.MessageType}", nameof(envelope));

            Command = command;
        }

        public new ILocalCommandEnvelope Envelope => (ILocalCommandEnvelope) base.Envelope;
        public new LocalCommandInfo Info => (LocalCommandInfo) base.Info;
        public TCommand Command { get; }
        public Type CommandType => Envelope.Info.MessageType;
        public Id CommandId => Envelope.MessageId;
    }
}