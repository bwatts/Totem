using System.Threading;
using System.Threading.Tasks;

namespace Totem.Queues;

public interface IQueueCommandPipeline
{
    Id Id { get; }

    Task<IQueueCommandContext<IQueueCommand>> RunAsync(IQueueCommandEnvelope envelope, CancellationToken token);
}
