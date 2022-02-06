using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Totem.Core;

namespace Totem.Local;

public class LocalQueryDispatcherMiddleware : ILocalQueryMiddleware
{
    delegate Task ContextHandler(ILocalQueryContext<ILocalQuery> context, CancellationToken cancellationToken);

    readonly ConcurrentDictionary<Type, ContextHandler> _handlersByContextType = new();
    readonly IServiceProvider _services;
    readonly IQueryReportClient _reportClient;

    public LocalQueryDispatcherMiddleware(IServiceProvider services, IQueryReportClient reportClient)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _reportClient = reportClient ?? throw new ArgumentNullException(nameof(reportClient));
    }

    public async Task InvokeAsync(ILocalQueryContext<ILocalQuery> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        if(context.Info.Result.IsReport)
        {
            await _reportClient.LoadResultAsync(context, cancellationToken);
        }
        else if(!context.QueryType.Contexts.TryGet(context.InterfaceType, out var queryContext) || !queryContext.HasHandler)
        {
            context.AddError(RuntimeErrors.QueryNotHandled);
        }
        else
        {
            var handler = _handlersByContextType.GetOrAdd(context.InterfaceType, _ => CompileHandler(context));

            await handler(context, cancellationToken);
        }

        if(!context.HasErrors)
        {
            await next();
        }
    }

    ContextHandler CompileHandler(ILocalQueryContext<ILocalQuery> context)
    {
        // (context, cancellationToken) => HandleAsync<TQuery>(context, cancellationToken)

        var contextParameter = Expression.Parameter(typeof(ILocalQueryContext<ILocalQuery>), "context");
        var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
        var call = Expression.Call(
            Expression.Constant(this),
            nameof(HandleAsync),
            new[] { context.QueryType.DeclaredType },
            contextParameter,
            cancellationTokenParameter);

        var lambda = Expression.Lambda<ContextHandler>(call, contextParameter, cancellationTokenParameter);

        return lambda.Compile();
    }

    async Task HandleAsync<TQuery>(ILocalQueryContext<ILocalQuery> context, CancellationToken cancellationToken)
        where TQuery : ILocalQuery
    {
        using var scope = _services.CreateScope();
        var handler = scope.ServiceProvider.GetService<ILocalQueryHandler<TQuery>>();

        if(handler is null)
        {
            context.AddError(RuntimeErrors.QueryHandlerNotFound);
            return;
        }

        await handler.HandleAsync((ILocalQueryContext<TQuery>) context, cancellationToken);
    }
}
