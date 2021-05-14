using System.Threading;
using System.Threading.Tasks;

namespace Totem.Events
{
    public interface IEventPipeline
    {
        Id Id { get; }

        Task<IEventContext<IEvent>> RunAsync(IEventEnvelope point, CancellationToken cancellationToken);
    }
}