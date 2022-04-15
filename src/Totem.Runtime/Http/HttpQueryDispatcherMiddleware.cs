using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Http;

public class HttpQueryDispatcherMiddleware : IHttpQueryMiddleware
{
    delegate Task ContextHandler(IHttpQueryContext<IHttpQuery> context, CancellationToken cancellationToken);

    readonly ConcurrentDictionary<Type, ContextHandler> _handlersByContextType = new();
    readonly IServiceProvider _services;

    public HttpQueryDispatcherMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public async Task InvokeAsync(IHttpQueryContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        if(!context.QueryType.Contexts.TryGet(context.InterfaceType, out var queryContext))
        {
            context.AddError(RuntimeErrors.QueryNotHandled);
            return;
        }

        var handler = _handlersByContextType.GetOrAdd(context.InterfaceType, _ => CompileHandler(context));

        await handler(context, cancellationToken);

        if(!context.HasErrors)
        {
            await next();
        }
    }

    ContextHandler CompileHandler(IHttpQueryContext<IHttpQuery> context)
    {
        // (context, cancellationToken) => HandleAsync<TQuery>(context, cancellationToken)

        var contextParameter = Expression.Parameter(typeof(IHttpQueryContext<IHttpQuery>), "context");
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

    async Task HandleAsync<TQuery>(IHttpQueryContext<IHttpQuery> context, CancellationToken cancellationToken)
        where TQuery : IHttpQuery
    {
        using var scope = _services.CreateScope();
        var handler = scope.ServiceProvider.GetService<IHttpQueryHandler<TQuery>>();

        if(handler is null)
        {
            context.AddError(RuntimeErrors.QueryHandlerNotFound);
            return;
        }

        await handler.HandleAsync((IHttpQueryContext<TQuery>) context, cancellationToken);
    }
}
