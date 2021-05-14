using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Queries
{
    public class ClientQueryMiddleware : IClientQueryMiddleware
    {
        readonly Func<IClientQueryContext<IQuery>, Func<Task>, CancellationToken, Task> _middleware;

        public ClientQueryMiddleware(Func<IClientQueryContext<IQuery>, Func<Task>, CancellationToken, Task> middleware) =>
            _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

        public Task InvokeAsync(IClientQueryContext<IQuery> context, Func<Task> next, CancellationToken cancellationToken) =>
            _middleware(context, next, cancellationToken);
    }

    public class ClientQueryMiddleware<TService> : IClientQueryMiddleware
        where TService : IClientQueryMiddleware
    {
        readonly IServiceProvider _services;

        public ClientQueryMiddleware(IServiceProvider services) =>
            _services = services ?? throw new ArgumentNullException(nameof(services));

        public Task InvokeAsync(IClientQueryContext<IQuery> context, Func<Task> next, CancellationToken cancellationToken) =>
            _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
    }
}