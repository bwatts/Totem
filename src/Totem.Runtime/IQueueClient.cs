using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem
{
    public interface IQueueClient
    {
        Task EnqueueAsync(IQueueEnvelope envelope, CancellationToken cancellationToken);
        Task EnqueueAsync(IEnumerable<IQueueEnvelope> envelopes, CancellationToken cancellationToken);
    }
}