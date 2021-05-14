using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using Totem.Core;

namespace Totem.Queues
{
    public class QueueContextFactory : IQueueContextFactory
    {
        delegate IQueueContext<IQueueCommand> TypeFactory(Id pipelineId, IQueueEnvelope envelope);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByCommandType = new();

        public IQueueContext<IQueueCommand> Create(Id pipelineId, IQueueEnvelope envelope)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            var factory = _factoriesByCommandType.GetOrAdd(envelope.MessageType, CompileFactory);

            return factory(pipelineId, envelope);
        }

        TypeFactory CompileFactory(Type commandType)
        {
            // (pipelineId, command) => new QueueContext<TCommand>(pipelineId, envelope)

            var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
            var envelopeParameter = Expression.Parameter(typeof(IQueueEnvelope), "envelope");
            var constructor = typeof(QueueContext<>).MakeGenericType(commandType).GetConstructors().Single();

            var lambda = Expression.Lambda<TypeFactory>(
                Expression.New(constructor, pipelineIdParameter, envelopeParameter),
                pipelineIdParameter,
                envelopeParameter);

            return lambda.Compile();
        }
    }
}