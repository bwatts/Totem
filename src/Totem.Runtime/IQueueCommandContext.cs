using System;
using Totem.Core;
using Totem.Queues;

namespace Totem
{
    public interface IQueueCommandContext<out TCommand> : IMessageContext
        where TCommand : IQueueCommand
    {
        new IQueueCommandEnvelope Envelope { get; }
        new QueueCommandInfo Info { get; }
        TCommand Command { get; }
        Type CommandType { get; }
        Id CommandId { get; }
        Text QueueName { get; }
    }
}