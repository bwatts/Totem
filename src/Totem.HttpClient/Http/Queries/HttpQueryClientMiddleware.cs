using Microsoft.Extensions.DependencyInjection;

namespace Totem.Http.Queries;

public class HttpQueryClientMiddleware : IHttpQueryClientMiddleware
{
    readonly Func<IHttpQueryClientContext<IHttpQuery>, Func<Task>, CancellationToken, Task> _middleware;

    public HttpQueryClientMiddleware(Func<IHttpQueryClientContext<IHttpQuery>, Func<Task>, CancellationToken, Task> middleware) =>
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

    public Task InvokeAsync(IHttpQueryClientContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken) =>
        _middleware(context, next, cancellationToken);
}

public class HttpQueryClientMiddleware<TService> : IHttpQueryClientMiddleware
    where TService : IHttpQueryClientMiddleware
{
    readonly IServiceProvider _services;

    public HttpQueryClientMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task InvokeAsync(IHttpQueryClientContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken) =>
        _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
}
