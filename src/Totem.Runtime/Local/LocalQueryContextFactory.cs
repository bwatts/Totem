using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Map;

namespace Totem.Local;

public class LocalQueryContextFactory : ILocalQueryContextFactory
{
    delegate ILocalQueryContext<ILocalQuery> TypeFactory(Id pipelineId, ILocalQueryEnvelope envelope);

    readonly ConcurrentDictionary<QueryType, TypeFactory> _factoriesByQueryType = new();
    readonly RuntimeMap _map;

    public LocalQueryContextFactory(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    public ILocalQueryContext<ILocalQuery> Create(Id pipelineId, ILocalQueryEnvelope envelope)
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
        var envelopeParameter = Expression.Parameter(typeof(ILocalQueryEnvelope), "envelope");
        var constructor = typeof(LocalQueryContext<>)
            .MakeGenericType(queryType.DeclaredType)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single();

        var lambda = Expression.Lambda<TypeFactory>(
            Expression.New(constructor, pipelineIdParameter, envelopeParameter),
            pipelineIdParameter,
            envelopeParameter);

        return lambda.Compile();
    }
}
