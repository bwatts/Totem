using Microsoft.Extensions.DependencyInjection;

namespace Totem.Commands;

public class ClientCommandMiddleware : IClientCommandMiddleware
{
    readonly Func<IClientCommandContext<IHttpCommand>, Func<Task>, CancellationToken, Task> _middleware;

    public ClientCommandMiddleware(Func<IClientCommandContext<IHttpCommand>, Func<Task>, CancellationToken, Task> middleware) =>
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

    public Task InvokeAsync(IClientCommandContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
        _middleware(context, next, cancellationToken);
}

public class ClientCommandMiddleware<TService> : IClientCommandMiddleware
    where TService : IClientCommandMiddleware
{
    readonly IServiceProvider _services;

    public ClientCommandMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task InvokeAsync(IClientCommandContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
        _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
}
