using System.Threading;
using System.Threading.Tasks;

namespace Totem.Routes
{
    public interface IRouteStore
    {
        Task<IRouteStoreTransaction> StartTransactionAsync(IRouteContext<IEvent> context, CancellationToken cancellationToken);
        Task CommitAsync(IRouteStoreTransaction transacion, CancellationToken cancellationToken);
    }
}