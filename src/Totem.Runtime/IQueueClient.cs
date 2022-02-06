using Totem.Queues;

namespace Totem;

public interface IQueueClient
{
    Task EnqueueAsync(IQueueCommandEnvelope envelope, CancellationToken cancellationToken);
    Task EnqueueAsync(IEnumerable<IQueueCommandEnvelope> envelopes, CancellationToken cancellationToken);
}
