using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Totem.Routes.Dispatch
{
    public class RouteDispatcher : IRouteDispatcher
    {
        readonly ConcurrentDictionary<Type, RouteTypeDispatcher> _dispatchersByRouteType = new();
        readonly IServiceProvider _services;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;
        readonly IRouteSettings _settings;
        readonly IRoutePipeline _pipeline;

        public RouteDispatcher(IServiceProvider services, ILoggerFactory loggerFactory, IRouteSettings settings, IRoutePipeline pipeline)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<RouteDispatcher>();
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        }

        public IEnumerable<IRouteSubscriber> CallRoute(Type routeType, IEvent e)
        {
            if(routeType == null)
                throw new ArgumentNullException(nameof(routeType));

            if(e == null)
                throw new ArgumentNullException(nameof(e));
            
            if(!typeof(IRoute).IsAssignableFrom(routeType))
                throw new ArgumentOutOfRangeException(nameof(routeType), $"Type {routeType} does not implement {typeof(IRoute)}");

            var dispatcher = _dispatchersByRouteType.GetOrAdd(routeType, CreateTypeDispatcher);

            return dispatcher.CallRoute(e);
        }

        public Task CallWhenAsync(IRoute route, IRouteContext<IEvent> context, CancellationToken cancellationToken)
        {
            if(route == null)
                throw new ArgumentNullException(nameof(route));

            if(context == null)
                throw new ArgumentNullException(nameof(context));

            var dispatcher = _dispatchersByRouteType.GetOrAdd(route.GetType(), CreateTypeDispatcher);

            return dispatcher.CallWhenAsync(route, context, cancellationToken);
        }

        RouteTypeDispatcher CreateTypeDispatcher(Type routeType) =>
            new(_services, _logger, routeType, routeId =>
                new RouteSubscriber(_loggerFactory, _settings, _pipeline, new RouteAddress(routeType, routeId)));
    }
}