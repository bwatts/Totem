using Microsoft.Extensions.DependencyInjection;

namespace Totem.Local;

public class LocalQueryMiddleware : ILocalQueryMiddleware
{
    readonly Func<ILocalQueryContext<ILocalQuery>, Func<Task>, CancellationToken, Task> _middleware;

    public LocalQueryMiddleware(Func<ILocalQueryContext<ILocalQuery>, Func<Task>, CancellationToken, Task> middleware) =>
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

    public Task InvokeAsync(ILocalQueryContext<ILocalQuery> context, Func<Task> next, CancellationToken cancellationToken) =>
        _middleware(context, next, cancellationToken);
}

public class LocalQueryMiddleware<TService> : ILocalQueryMiddleware
    where TService : ILocalQueryMiddleware
{
    readonly IServiceProvider _services;

    public LocalQueryMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task InvokeAsync(ILocalQueryContext<ILocalQuery> context, Func<Task> next, CancellationToken cancellationToken) =>
        _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
}
