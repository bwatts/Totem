using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Totem.Events
{
    public class EventHandlerMiddleware : IEventMiddleware
    {
        delegate IEventHandler<IEvent> TypeFactory(IServiceProvider services);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByEventType = new();
        readonly ILogger _logger;
        readonly IServiceProvider _services;

        public EventHandlerMiddleware(ILogger<EventHandlerMiddleware> logger, IServiceProvider services)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public async Task InvokeAsync(IEventContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(next == null)
                throw new ArgumentNullException(nameof(next));

            await TryInvokeHandlerAsync(context, cancellationToken);

            await next();
        }

        async Task TryInvokeHandlerAsync(IEventContext<IEvent> context, CancellationToken cancellationToken)
        {
            var factory = _factoriesByEventType.GetOrAdd(context.EventType, CompileFactory);

            var handler = factory?.Invoke(_services);

            if(handler != null)
            {
                _logger.LogTrace("[event] Handle {EventType}.{EventId} from {TimelineType}.{TimelineId}@{TimelineVersion}", context.EventType, context.EventId, context.TimelineType, context.TimelineId, context.TimelineVersion);

                await handler.HandleAsync(context, cancellationToken);
            }
        }

        TypeFactory CompileFactory(Type eventType)
        {
            // services => new TypedHandler<TEvent>(services)

            var parameter = Expression.Parameter(typeof(IServiceProvider), "services");
            var constructor = typeof(TypedHandler<>).MakeGenericType(eventType).GetConstructors().Single();
            var construct = Expression.New(constructor, parameter);

            return Expression.Lambda<TypeFactory>(construct, parameter).Compile();
        }

        class TypedHandler<TEvent> : IEventHandler<IEvent> where TEvent : IEvent
        {
            readonly IServiceProvider _services;

            public TypedHandler(IServiceProvider services) =>
                _services = services;

            public async Task HandleAsync(IEventContext<IEvent> context, CancellationToken cancellationToken)
            {
                var handler = _services.GetService<IEventHandler<TEvent>>();

                if(handler != null)
                {
                    await handler.HandleAsync((IEventContext<TEvent>) context, cancellationToken);
                }
            }
        }
    }
}