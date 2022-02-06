using Totem.Core;

namespace Totem.Queues;

public interface IQueueCommandEnvelope : ICommandEnvelope
{
    new IQueueCommand Message { get; }
    new QueueCommandInfo Info { get; }
}
