using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Routes
{
    public sealed class RouteStoreTransaction : IRouteStoreTransaction
    {
        readonly IRouteStore _store;
        readonly CancellationToken _cancellationToken;

        public RouteStoreTransaction(IRouteStore store, IRoute route, IRouteContext<IEvent> context, CancellationToken cancellationToken)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            Route = route ?? throw new ArgumentNullException(nameof(route));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _cancellationToken = cancellationToken;
        }

        public IRoute Route { get; }
        public IRouteContext<IEvent> Context { get; }

        public async Task CommitAsync()
        {
            if(!_cancellationToken.IsCancellationRequested)
            {
                await _store.CommitAsync(this, _cancellationToken);
            }
        }

        public void Dispose()
        { }
    }
}