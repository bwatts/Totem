using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Map;

namespace Totem.Queues;

public class QueueCommandContextFactory : IQueueCommandContextFactory
{
    delegate IQueueCommandContext<IQueueCommand> TypeFactory(Id pipelineId, IQueueCommandEnvelope envelope);

    readonly ConcurrentDictionary<CommandType, TypeFactory> _factoriesByCommandType = new();
    readonly RuntimeMap _map;

    public QueueCommandContextFactory(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    public IQueueCommandContext<IQueueCommand> Create(Id pipelineId, IQueueCommandEnvelope envelope)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        if(!_map.Commands.TryGet(envelope.MessageKey.DeclaredType, out var commandType))
            throw new ArgumentException($"Expected known command of type {envelope.MessageKey.DeclaredType}", nameof(envelope));

        var factory = _factoriesByCommandType.GetOrAdd(commandType, CompileFactory);

        return factory(pipelineId, envelope);
    }

    TypeFactory CompileFactory(CommandType commandType)
    {
        // (pipelineId, command) => new QueueContext<TCommand>(pipelineId, envelope, commandType)

        var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
        var envelopeParameter = Expression.Parameter(typeof(IQueueCommandEnvelope), "envelope");
        var constructor = typeof(QueueCommandContext<>)
            .MakeGenericType(commandType.DeclaredType)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single();

        var lambda = Expression.Lambda<TypeFactory>(
            Expression.New(constructor, pipelineIdParameter, envelopeParameter, Expression.Constant(commandType)),
            pipelineIdParameter,
            envelopeParameter);

        return lambda.Compile();
    }
}
