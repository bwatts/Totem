using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using Totem.Core;

namespace Totem.Queries
{
    public class QueryContextFactory : IQueryContextFactory
    {
        delegate IQueryContext<IQuery> TypeFactory(Id pipelineId, IQueryEnvelope envelope);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByQueryType = new();

        public IQueryContext<IQuery> Create(Id pipelineId, IQueryEnvelope envelope)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            var factory = _factoriesByQueryType.GetOrAdd(envelope.MessageType, CompileFactory);

            return factory(pipelineId, envelope);
        }

        TypeFactory CompileFactory(Type queryType)
        {
            // (pipelineId, query) => new QueryContext<TQuery>(pipelineId, _apmContext, (TQuery) query)

            var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
            var envelopeParameter = Expression.Parameter(typeof(IQueryEnvelope), "envelope");
            var constructor = typeof(QueryContext<>).MakeGenericType(queryType).GetConstructors().Single();

            var lambda = Expression.Lambda<TypeFactory>(
                Expression.New(constructor, pipelineIdParameter, envelopeParameter),
                pipelineIdParameter,
                envelopeParameter);

            return lambda.Compile();
        }
    }
}