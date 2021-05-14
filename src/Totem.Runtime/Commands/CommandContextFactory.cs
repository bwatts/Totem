using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using Totem.Core;

namespace Totem.Commands
{
    public class CommandContextFactory : ICommandContextFactory
    {
        delegate ICommandContext<ICommand> TypeFactory(Id pipelineId, ICommandEnvelope envelope);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByCommandType = new();

        public ICommandContext<ICommand> Create(Id pipelineId, ICommandEnvelope envelope)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            var factory = _factoriesByCommandType.GetOrAdd(envelope.MessageType, CompileFactory);

            return factory(pipelineId, envelope);
        }

        TypeFactory CompileFactory(Type commandType)
        {
            // (pipelineId, command) => new CommandContext<TCommand>(pipelineId, envelope)

            var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
            var envelopeParameter = Expression.Parameter(typeof(ICommandEnvelope), "envelope");
            var constructor = typeof(CommandContext<>).MakeGenericType(commandType).GetConstructors().Single();

            var lambda = Expression.Lambda<TypeFactory>(
                Expression.New(constructor, pipelineIdParameter, envelopeParameter),
                pipelineIdParameter,
                envelopeParameter);

            return lambda.Compile();
        }
    }
}