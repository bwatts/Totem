using System.Collections.Concurrent;
using System.Linq.Expressions;
using Totem.Http;

namespace Totem.Queries;

public class ClientQueryContextFactory : IClientQueryContextFactory
{
    delegate IClientQueryContext<IHttpQuery> TypeFactory(Id pipelineId, IHttpQueryEnvelope envelope);

    readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByQueryType = new();

    public IClientQueryContext<IHttpQuery> Create(Id pipelineId, IHttpQueryEnvelope envelope)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        var factory = _factoriesByQueryType.GetOrAdd(envelope.MessageKey.DeclaredType, CompileFactory);

        return factory(pipelineId, envelope);
    }

    TypeFactory CompileFactory(Type queryType)
    {
        // (pipelineId, envelope) => new ClientQueryContext<TCommand>(pipelineId, envelope)

        var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
        var envelopeParameter = Expression.Parameter(typeof(IHttpQueryEnvelope), "envelope");
        var constructor = typeof(ClientQueryContext<>).MakeGenericType(queryType).GetConstructors().Single();

        var lambda = Expression.Lambda<TypeFactory>(
            Expression.New(constructor, pipelineIdParameter, envelopeParameter),
            pipelineIdParameter,
            envelopeParameter);

        return lambda.Compile();
    }
}
