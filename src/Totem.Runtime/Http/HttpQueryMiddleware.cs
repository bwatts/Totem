using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Http
{
    public class HttpQueryMiddleware : IHttpQueryMiddleware
    {
        readonly Func<IHttpQueryContext<IHttpQuery>, Func<Task>, CancellationToken, Task> _middleware;

        public HttpQueryMiddleware(Func<IHttpQueryContext<IHttpQuery>, Func<Task>, CancellationToken, Task> middleware) =>
            _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

        public Task InvokeAsync(IHttpQueryContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken) =>
            _middleware(context, next, cancellationToken);
    }

    public class HttpQueryMiddleware<TService> : IHttpQueryMiddleware
        where TService : IHttpQueryMiddleware
    {
        readonly IServiceProvider _services;

        public HttpQueryMiddleware(IServiceProvider services) =>
            _services = services ?? throw new ArgumentNullException(nameof(services));

        public Task InvokeAsync(IHttpQueryContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken) =>
            _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
    }
}