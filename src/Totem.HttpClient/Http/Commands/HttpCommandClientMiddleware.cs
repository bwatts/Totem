using Microsoft.Extensions.DependencyInjection;

namespace Totem.Http.Commands;

public class HttpCommandClientMiddleware : IHttpCommandClientMiddleware
{
    readonly Func<IHttpCommandClientContext<IHttpCommand>, Func<Task>, CancellationToken, Task> _middleware;

    public HttpCommandClientMiddleware(Func<IHttpCommandClientContext<IHttpCommand>, Func<Task>, CancellationToken, Task> middleware) =>
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

    public Task InvokeAsync(IHttpCommandClientContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
        _middleware(context, next, cancellationToken);
}

public class HttpCommandClientMiddleware<TService> : IHttpCommandClientMiddleware
    where TService : IHttpCommandClientMiddleware
{
    readonly IServiceProvider _services;

    public HttpCommandClientMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task InvokeAsync(IHttpCommandClientContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
        _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
}
