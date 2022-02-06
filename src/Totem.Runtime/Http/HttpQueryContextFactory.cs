using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Map;

namespace Totem.Http;

public class HttpQueryContextFactory : IHttpQueryContextFactory
{
    delegate IHttpQueryContext<IHttpQuery> TypeFactory(Id pipelineId, IHttpQueryEnvelope envelope);

    readonly ConcurrentDictionary<QueryType, TypeFactory> _factoriesByQueryType = new();
    readonly RuntimeMap _map;

    public HttpQueryContextFactory(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    public IHttpQueryContext<IHttpQuery> Create(Id pipelineId, IHttpQueryEnvelope envelope)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        if(!_map.Queries.TryGet(envelope.MessageKey.DeclaredType, out var queryType))
            throw new ArgumentException($"Expected known query of type {envelope.MessageKey.DeclaredType}", nameof(envelope));

        var factory = _factoriesByQueryType.GetOrAdd(queryType, CompileFactory);

        return factory(pipelineId, envelope);
    }

    TypeFactory CompileFactory(QueryType queryType)
    {
        // (pipelineId, query) => new QueryContext<TQuery>(pipelineId, envelope, queryType)

        var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
        var envelopeParameter = Expression.Parameter(typeof(IHttpQueryEnvelope), "envelope");
        var constructor = typeof(HttpQueryContext<>)
            .MakeGenericType(queryType.DeclaredType)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single();

        var lambda = Expression.Lambda<TypeFactory>(
            Expression.New(constructor, pipelineIdParameter, envelopeParameter, Expression.Constant(queryType)),
            pipelineIdParameter,
            envelopeParameter);

        return lambda.Compile();
    }
}
