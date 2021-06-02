using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace Totem.Http
{
    public class HttpQueryContextFactory : IHttpQueryContextFactory
    {
        delegate IHttpQueryContext<IHttpQuery> TypeFactory(Id pipelineId, IHttpQueryEnvelope envelope);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByQueryType = new();

        public IHttpQueryContext<IHttpQuery> Create(Id pipelineId, IHttpQueryEnvelope envelope)
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
            var envelopeParameter = Expression.Parameter(typeof(IHttpQueryEnvelope), "envelope");
            var constructor = typeof(HttpQueryContext<>).MakeGenericType(queryType).GetConstructors().Single();

            var lambda = Expression.Lambda<TypeFactory>(
                Expression.New(constructor, pipelineIdParameter, envelopeParameter),
                pipelineIdParameter,
                envelopeParameter);

            return lambda.Compile();
        }
    }
}