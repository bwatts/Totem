using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using Totem.Http;

namespace Totem.Commands;

public class ClientCommandContextFactory : IClientCommandContextFactory
{
    delegate IClientCommandContext<IHttpCommand> TypeFactory(Id pipelineId, IHttpCommandEnvelope envelope);

    readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByCommandType = new();

    public IClientCommandContext<IHttpCommand> Create(Id pipelineId, IHttpCommandEnvelope envelope)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        var factory = _factoriesByCommandType.GetOrAdd(envelope.MessageKey.DeclaredType, CompileFactory);

        return factory(pipelineId, envelope);
    }

    TypeFactory CompileFactory(Type commandType)
    {
        // (pipelineId, envelope) => new ClientCommandContext<TCommand>(pipelineId, envelope)

        var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
        var envelopeParameter = Expression.Parameter(typeof(IHttpCommandEnvelope), "envelope");
        var constructor = typeof(ClientCommandContext<>).MakeGenericType(commandType).GetConstructors().Single();

        var lambda = Expression.Lambda<TypeFactory>(
            Expression.New(constructor, pipelineIdParameter, envelopeParameter),
            pipelineIdParameter,
            envelopeParameter);

        return lambda.Compile();
    }
}
