using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using Totem.Core;

namespace Totem.Routes
{
    public class RouteContextFactory : IRouteContextFactory
    {
        delegate IRouteContext<IEvent> TypeFactory(Id pipelineId, IEventEnvelope envelope, IRouteAddress address);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByEventType = new();

        public IRouteContext<IEvent> Create(Id pipelineId, IEventEnvelope envelope, IRouteAddress address)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            var factory = _factoriesByEventType.GetOrAdd(envelope.Info.MessageType, CompileFactory);

            return factory(pipelineId, envelope, address);
        }

        TypeFactory CompileFactory(Type eventType)
        {
            // (pipelineId, point, address) => new RouteContext<TEvent>(pipelineId, envelope, address)

            var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
            var envelopeParameter = Expression.Parameter(typeof(IEventEnvelope), "envelope");
            var addressParameter = Expression.Parameter(typeof(IRouteAddress), "address");
            var constructor = typeof(RouteContext<>).MakeGenericType(eventType).GetConstructors().Single();

            var lambda = Expression.Lambda<TypeFactory>(
                Expression.New(constructor, pipelineIdParameter, envelopeParameter, addressParameter),
                pipelineIdParameter,
                envelopeParameter,
                addressParameter);

            return lambda.Compile();
        }
    }
}