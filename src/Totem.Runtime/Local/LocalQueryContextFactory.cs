using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace Totem.Local
{
    public class LocalQueryContextFactory : ILocalQueryContextFactory
    {
        delegate ILocalQueryContext<ILocalQuery> TypeFactory(Id pipelineId, ILocalQueryEnvelope envelope);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByQueryType = new();

        public ILocalQueryContext<ILocalQuery> Create(Id pipelineId, ILocalQueryEnvelope envelope)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            var factory = _factoriesByQueryType.GetOrAdd(envelope.Info.MessageType, CompileFactory);

            return factory(pipelineId, envelope);
        }

        TypeFactory CompileFactory(Type queryType)
        {
            // (pipelineId, query) => new QueryContext<TQuery>(pipelineId, _apmContext, (TQuery) query)

            var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
            var envelopeParameter = Expression.Parameter(typeof(ILocalQueryEnvelope), "envelope");
            var constructor = typeof(LocalQueryContext<>).MakeGenericType(queryType).GetConstructors().Single();

            var lambda = Expression.Lambda<TypeFactory>(
                Expression.New(constructor, pipelineIdParameter, envelopeParameter),
                pipelineIdParameter,
                envelopeParameter);

            return lambda.Compile();
        }
    }
}