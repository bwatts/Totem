using System;
using Totem.Core;

namespace Totem.Queues
{
    public class QueueContext<TCommand> : MessageContext, IQueueContext<TCommand>
        where TCommand : IQueueCommand
    {
        public QueueContext(Id pipelineId, IQueueEnvelope envelope) : base(pipelineId, envelope)
        {
            Envelope = envelope;

            if(envelope.Message is not TCommand command)
                throw new ArgumentException($"Expected command type {typeof(TCommand)} but received {envelope.MessageType}", nameof(envelope));

            Command = command;
        }

        public new IQueueEnvelope Envelope { get; }
        public TCommand Command { get; }
        public Type CommandType => Envelope.MessageType;
        public Id CommandId => Envelope.MessageId;
        public Text QueueName => Envelope.QueueName;
    }
}