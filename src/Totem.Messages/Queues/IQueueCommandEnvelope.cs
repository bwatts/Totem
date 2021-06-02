using Totem.Core;

namespace Totem.Queues
{
    public interface IQueueCommandEnvelope : IMessageEnvelope
    {
        new IQueueCommand Message { get; }
        new QueueCommandInfo Info { get; }
    }
}