using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Queries
{
    public class QueryMiddleware : IQueryMiddleware
    {
        readonly Func<IQueryContext<IQuery>, Func<Task>, CancellationToken, Task> _middleware;

        public QueryMiddleware(Func<IQueryContext<IQuery>, Func<Task>, CancellationToken, Task> middleware) =>
            _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

        public Task InvokeAsync(IQueryContext<IQuery> context, Func<Task> next, CancellationToken cancellationToken) =>
            _middleware(context, next, cancellationToken);
    }

    public class QueryMiddleware<TService> : IQueryMiddleware
        where TService : IQueryMiddleware
    {
        readonly IServiceProvider _services;

        public QueryMiddleware(IServiceProvider services) =>
            _services = services ?? throw new ArgumentNullException(nameof(services));

        public Task InvokeAsync(IQueryContext<IQuery> context, Func<Task> next, CancellationToken cancellationToken) =>
            _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
    }
}