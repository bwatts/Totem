using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Http;

public class HttpCommandMiddleware : IHttpCommandMiddleware
{
    readonly Func<IHttpCommandContext<IHttpCommand>, Func<Task>, CancellationToken, Task> _middleware;

    public HttpCommandMiddleware(Func<IHttpCommandContext<IHttpCommand>, Func<Task>, CancellationToken, Task> middleware) =>
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

    public Task InvokeAsync(IHttpCommandContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
        _middleware(context, next, cancellationToken);
}

public class HttpCommandMiddleware<TService> : IHttpCommandMiddleware
    where TService : IHttpCommandMiddleware
{
    readonly IServiceProvider _services;

    public HttpCommandMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task InvokeAsync(IHttpCommandContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
        _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
}
