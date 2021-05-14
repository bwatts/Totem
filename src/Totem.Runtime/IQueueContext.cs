using System;
using Totem.Core;

namespace Totem
{
    public interface IQueueContext<out TCommand> : IMessageContext
        where TCommand : IQueueCommand
    {
        new IQueueEnvelope Envelope { get; }
        TCommand Command { get; }
        Type CommandType { get; }
        Id CommandId { get; }
        Text QueueName { get; }
    }
}