using System;
using Totem.Core;

namespace Totem.Queues
{
    public class QueueCommandContext<TCommand> : MessageContext, IQueueCommandContext<TCommand>
        where TCommand : IQueueCommand
    {
        public QueueCommandContext(Id pipelineId, IQueueCommandEnvelope envelope) : base(pipelineId, envelope)
        {
            if(envelope.Message is not TCommand command)
                throw new ArgumentException($"Expected command type {typeof(TCommand)} but received {envelope.Info.MessageType}", nameof(envelope));

            Command = command;
        }

        public new IQueueCommandEnvelope Envelope => (IQueueCommandEnvelope) base.Envelope;
        public new QueueCommandInfo Info => (QueueCommandInfo) base.Info;
        public TCommand Command { get; }
        public Type CommandType => Info.MessageType;
        public Id CommandId => Envelope.MessageId;
        public Text QueueName => Info.QueueName;

        MessageInfo IMessageContext.Info => Info;
    }
}