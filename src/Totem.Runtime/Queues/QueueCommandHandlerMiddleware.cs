using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Totem.Queues
{
    public class QueueCommandHandlerMiddleware : IQueueCommandMiddleware
    {
        delegate IQueueCommandHandler<IQueueCommand> TypeFactory(IServiceProvider services);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByCommandType = new();
        readonly ILogger _logger;
        readonly IServiceProvider _services;

        public QueueCommandHandlerMiddleware(ILogger<QueueCommandHandlerMiddleware> logger, IServiceProvider services)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public async Task InvokeAsync(IQueueCommandContext<IQueueCommand> context, Func<Task> next, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(next == null)
                throw new ArgumentNullException(nameof(next));

            await TryInvokeHandlerAsync(context, cancellationToken);

            await next();
        }

        async Task TryInvokeHandlerAsync(IQueueCommandContext<IQueueCommand> context, CancellationToken cancellationToken)
        {
            var factory = _factoriesByCommandType.GetOrAdd(context.CommandType, CompileFactory);

            var handler = factory.Invoke(_services);

            if(handler != null)
            {
                _logger.LogTrace("[queue {QueueName}] Handle {@CommandType}.{@CommandId}", context.QueueName, context.CommandType, context.CommandId);

                await handler.HandleAsync(context, cancellationToken);
            }
        }

        TypeFactory CompileFactory(Type commandType)
        {
            // services => new TypedHandler<TCommand>(services)

            var parameter = Expression.Parameter(typeof(IServiceProvider), "services");
            var constructor = typeof(TypedHandler<>).MakeGenericType(commandType).GetConstructors().Single();
            var construct = Expression.New(constructor, parameter);

            return Expression.Lambda<TypeFactory>(construct, parameter).Compile();
        }

        class TypedHandler<TCommand> : IQueueCommandHandler<IQueueCommand> where TCommand : IQueueCommand
        {
            readonly IServiceProvider _services;

            public TypedHandler(IServiceProvider services) =>
                _services = services;

            public async Task HandleAsync(IQueueCommandContext<IQueueCommand> context, CancellationToken cancellationToken)
            {
                var handler = _services.GetService<IQueueCommandHandler<TCommand>>();

                if(handler != null)
                {
                    await handler.HandleAsync((IQueueCommandContext<TCommand>) context, cancellationToken);
                }
            }
        }
    }
}