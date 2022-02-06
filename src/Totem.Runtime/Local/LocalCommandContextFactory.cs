using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Map;

namespace Totem.Local;

public class LocalCommandContextFactory : ILocalCommandContextFactory
{
    delegate ILocalCommandContext<ILocalCommand> TypeFactory(Id pipelineId, ILocalCommandEnvelope envelope);

    readonly ConcurrentDictionary<CommandType, TypeFactory> _factoriesByCommandType = new();
    readonly RuntimeMap _map;

    public LocalCommandContextFactory(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    public ILocalCommandContext<ILocalCommand> Create(Id pipelineId, ILocalCommandEnvelope envelope)
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
        // (pipelineId, command) => new CommandContext<TCommand>(pipelineId, envelope, commandType)

        var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
        var envelopeParameter = Expression.Parameter(typeof(ILocalCommandEnvelope), "envelope");
        var constructor = typeof(LocalCommandContext<>)
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
