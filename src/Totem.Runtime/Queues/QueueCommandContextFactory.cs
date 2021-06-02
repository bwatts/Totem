using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace Totem.Queues
{
    public class QueueCommandContextFactory : IQueueCommandContextFactory
    {
        delegate IQueueCommandContext<IQueueCommand> TypeFactory(Id pipelineId, IQueueCommandEnvelope envelope);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByCommandType = new();

        public IQueueCommandContext<IQueueCommand> Create(Id pipelineId, IQueueCommandEnvelope envelope)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            var factory = _factoriesByCommandType.GetOrAdd(envelope.Info.MessageType, CompileFactory);

            return factory(pipelineId, envelope);
        }

        TypeFactory CompileFactory(Type commandType)
        {
            // (pipelineId, command) => new QueueContext<TCommand>(pipelineId, envelope)

            var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
            var envelopeParameter = Expression.Parameter(typeof(IQueueCommandEnvelope), "envelope");
            var constructor = typeof(QueueCommandContext<>).MakeGenericType(commandType).GetConstructors().Single();

            var lambda = Expression.Lambda<TypeFactory>(
                Expression.New(constructor, pipelineIdParameter, envelopeParameter),
                pipelineIdParameter,
                envelopeParameter);

            return lambda.Compile();
        }
    }
}