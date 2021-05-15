using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Totem.Queries
{
    public class QueryHandlerMiddleware : IQueryMiddleware
    {
        delegate IQueryHandler<IQuery> TypeFactory(IServiceProvider services);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByQueryType = new();
        readonly ILogger _logger;
        readonly IServiceProvider _services;

        public QueryHandlerMiddleware(ILogger<QueryHandlerMiddleware> logger, IServiceProvider services)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public async Task InvokeAsync(IQueryContext<IQuery> context, Func<Task> next, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(next == null)
                throw new ArgumentNullException(nameof(next));

            await TryInvokeHandlerAsync(context, cancellationToken);

            await next();
        }

        async Task TryInvokeHandlerAsync(IQueryContext<IQuery> context, CancellationToken cancellationToken)
        {
            var factory = _factoriesByQueryType.GetOrAdd(context.QueryType, CompileFactory);

            var handler = factory.Invoke(_services);

            if(handler != null)
            {
                _logger.LogTrace("[query] Handle {@QueryType}.{@QueryId}", context.QueryType, context.QueryId);

                await handler.HandleAsync(context, cancellationToken);
            }
        }

        TypeFactory CompileFactory(Type queryType)
        {
            // services => new TypedHandler<TQuery>(services)

            var parameter = Expression.Parameter(typeof(IServiceProvider), "services");
            var constructor = typeof(TypedHandler<>).MakeGenericType(queryType).GetConstructors().Single();
            var construct = Expression.New(constructor, parameter);

            return Expression.Lambda<TypeFactory>(construct, parameter).Compile();
        }

        class TypedHandler<TQuery> : IQueryHandler<IQuery> where TQuery : IQuery
        {
            readonly IServiceProvider _services;

            public TypedHandler(IServiceProvider services) =>
                _services = services;

            public async Task HandleAsync(IQueryContext<IQuery> context, CancellationToken cancellationToken)
            {
                var handler = _services.GetService<IQueryHandler<TQuery>>();

                if(handler != null)
                {
                    await handler.HandleAsync((IQueryContext<TQuery>) context, cancellationToken);
                }
            }
        }
    }
}