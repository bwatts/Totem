using System.Threading;
using System.Threading.Tasks;
using Totem.Events;

namespace Totem.Routes
{
    public interface IRoutePipeline
    {
        Id Id { get; }

        Task<IRouteContext<IEvent>> RunAsync(IEventEnvelope envelope, IRouteAddress address, CancellationToken cancellationToken);
    }
}