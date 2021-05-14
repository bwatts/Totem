using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Routes.Dispatch
{
    public interface IRouteDispatcher
    {
        IEnumerable<IRouteSubscriber> CallRoute(Type routeType, IEvent e);

        Task CallWhenAsync(IRoute route, IRouteContext<IEvent> context, CancellationToken cancellationToken);
    }
}