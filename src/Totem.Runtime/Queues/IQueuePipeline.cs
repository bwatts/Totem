using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Queues
{
    public interface IQueuePipeline
    {
        Id Id { get; }

        Task<IQueueContext<IQueueCommand>> RunAsync(IQueueEnvelope envelope, CancellationToken token);
    }
}