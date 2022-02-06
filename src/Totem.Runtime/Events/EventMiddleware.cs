using Microsoft.Extensions.DependencyInjection;

namespace Totem.Events;

public class EventMiddleware : IEventMiddleware
{
    readonly Func<IEventContext<IEvent>, Func<Task>, CancellationToken, Task> _middleware;

    public EventMiddleware(Func<IEventContext<IEvent>, Func<Task>, CancellationToken, Task> middleware) =>
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

    public Task InvokeAsync(IEventContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken) =>
        _middleware(context, next, cancellationToken);
}

public class EventMiddleware<TService> : IEventMiddleware
    where TService : IEventMiddleware
{
    readonly IServiceProvider _services;

    public EventMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task InvokeAsync(IEventContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken) =>
        _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
}
