using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Totem.Map;

namespace Totem.Events;

public class EventHandlerMiddleware : IEventMiddleware
{
    delegate Task TypeHandler(IEventContext<IEvent> context, CancellationToken cancellationToken);

    readonly ConcurrentDictionary<EventType, TypeHandler> _handlersByEventType = new();
    readonly IServiceProvider _services;

    public EventHandlerMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public async Task InvokeAsync(IEventContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        var handler = _handlersByEventType.GetOrAdd(context.EventType, _ => CompileHandler(context));

        await handler(context, cancellationToken);

        await next();
    }

    TypeHandler CompileHandler(IEventContext<IEvent> context)
    {
        // (context, cancellationToken) => HandleAsync<TEvent>(context, cancellationToken)

        var contextParameter = Expression.Parameter(typeof(IEventContext<IEvent>), "context");
        var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
        var call = Expression.Call(
            Expression.Constant(this),
            nameof(HandleAsync),
            new[] { context.EventType.DeclaredType },
            contextParameter,
            cancellationTokenParameter);

        var lambda = Expression.Lambda<TypeHandler>(call, contextParameter, cancellationTokenParameter);

        return lambda.Compile();
    }

    async Task HandleAsync<TEvent>(IEventContext<IEvent> context, CancellationToken cancellationToken)
        where TEvent : IEvent
    {
        using var scope = _services.CreateScope();

        foreach(var handler in scope.ServiceProvider.GetServices<IEventHandler<TEvent>>())
        {
            await handler.HandleAsync((IEventContext<TEvent>) context, cancellationToken);
        }
    }
}
