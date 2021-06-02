using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Totem.Queues;

namespace Totem
{
    public interface IQueueCommandClient
    {
        Task EnqueueAsync(IQueueCommandEnvelope envelope, CancellationToken cancellationToken);
        Task EnqueueAsync(IEnumerable<IQueueCommandEnvelope> envelopes, CancellationToken cancellationToken);
    }
}