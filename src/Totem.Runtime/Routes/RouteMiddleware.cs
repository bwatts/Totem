using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Routes
{
    public class RouteMiddleware : IRouteMiddleware
    {
        readonly Func<IRouteContext<IEvent>, Func<Task>, CancellationToken, Task> _middleware;

        public RouteMiddleware(Func<IRouteContext<IEvent>, Func<Task>, CancellationToken, Task> middleware) =>
            _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

        public Task InvokeAsync(IRouteContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken) =>
            _middleware(context, next, cancellationToken);
    }

    public class RouteMiddleware<TService> : IRouteMiddleware
        where TService : IRouteMiddleware
    {
        readonly IServiceProvider _services;

        public RouteMiddleware(IServiceProvider services) =>
            _services = services ?? throw new ArgumentNullException(nameof(services));

        public Task InvokeAsync(IRouteContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken) =>
            _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
    }
}