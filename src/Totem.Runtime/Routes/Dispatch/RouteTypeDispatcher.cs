using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Totem.Routes.Dispatch
{
    internal class RouteTypeDispatcher
    {
        readonly Dictionary<Type, RouteMethod> _routesByEventType = new();
        readonly Dictionary<Type, WhenMethod> _whensByEventType = new();

        internal RouteTypeDispatcher(IServiceProvider services, ILogger logger, Type routeType, Func<Id, IRouteSubscriber> createSubscriber)
        {
            var compiler = new RouteTypeCompiler(services, logger, routeType, createSubscriber);

            _routesByEventType = compiler.RoutesByEventType;
            _whensByEventType = compiler.WhensByEventType;
        }

        internal IEnumerable<IRouteSubscriber> CallRoute(IEvent e)
        {
            if(e == null)
                throw new ArgumentNullException(nameof(e));

            if(_routesByEventType.TryGetValue(e.GetType(), out var route))
            {
                foreach(var subscriber in route.Call(e))
                {
                    yield return subscriber;
                }
            }
        }

        internal async Task CallWhenAsync(IRoute route, IRouteContext<IEvent> context, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(_whensByEventType.TryGetValue(context.EventType, out var when))
            {
                await when.CallAsync(route, context, cancellationToken);
            }
        }
    }
}