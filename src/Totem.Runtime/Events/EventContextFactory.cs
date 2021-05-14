using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace Totem.Events
{
    public class EventContextFactory : IEventContextFactory
    {
        delegate IEventContext<IEvent> TypeFactory(Id pipelineId, IEventEnvelope envelope);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByEventType = new();

        public IEventContext<IEvent> Create(Id pipelineId, IEventEnvelope envelope)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            var factory = _factoriesByEventType.GetOrAdd(envelope.MessageType, CompileFactory);

            return factory(pipelineId, envelope);
        }

        TypeFactory CompileFactory(Type eventType)
        {
            // (pipelineId, envelope) => new EventContext<TEvent>(pipelineId, envelope)

            var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
            var envelopeParameter = Expression.Parameter(typeof(IEventEnvelope), "envelope");
            var constructor = typeof(EventContext<>).MakeGenericType(eventType).GetConstructors().Single();

            var lambda = Expression.Lambda<TypeFactory>(
                Expression.New(constructor, pipelineIdParameter, envelopeParameter),
                pipelineIdParameter,
                envelopeParameter);

            return lambda.Compile();
        }
    }
}