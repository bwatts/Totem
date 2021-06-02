using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace Totem.Http
{
    public class HttpCommandContextFactory : IHttpCommandContextFactory
    {
        delegate IHttpCommandContext<IHttpCommand> TypeFactory(Id pipelineId, IHttpCommandEnvelope envelope);

        readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByCommandType = new();

        public IHttpCommandContext<IHttpCommand> Create(Id pipelineId, IHttpCommandEnvelope envelope)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            var factory = _factoriesByCommandType.GetOrAdd(envelope.Info.MessageType, CompileFactory);

            return factory(pipelineId, envelope);
        }

        TypeFactory CompileFactory(Type commandType)
        {
            // (pipelineId, command) => new CommandContext<TCommand>(pipelineId, envelope)

            var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
            var envelopeParameter = Expression.Parameter(typeof(IHttpCommandEnvelope), "envelope");
            var constructor = typeof(HttpCommandContext<>).MakeGenericType(commandType).GetConstructors().Single();

            var lambda = Expression.Lambda<TypeFactory>(
                Expression.New(constructor, pipelineIdParameter, envelopeParameter),
                pipelineIdParameter,
                envelopeParameter);

            return lambda.Compile();
        }
    }
}