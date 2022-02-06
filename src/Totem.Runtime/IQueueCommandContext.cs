using Totem.Core;
using Totem.Queues;

namespace Totem;

public interface IQueueCommandContext<out TCommand> : ICommandContext<TCommand>
    where TCommand : IQueueCommand
{
    new IQueueCommandEnvelope Envelope { get; }
    new QueueCommandInfo Info { get; }
    Text QueueName { get; }
}
