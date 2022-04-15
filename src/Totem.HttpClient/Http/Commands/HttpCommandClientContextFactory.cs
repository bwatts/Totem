namespace Totem.Http.Commands;

public class HttpCommandClientContextFactory : IHttpCommandClientContextFactory
{
    delegate IHttpCommandClientContext<IHttpCommand> TypeFactory(Id pipelineId, IHttpCommandEnvelope envelope);

    readonly ConcurrentDictionary<Type, TypeFactory> _factoriesByCommandType = new();

    public IHttpCommandClientContext<IHttpCommand> Create(Id pipelineId, IHttpCommandEnvelope envelope)
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
        var constructor = typeof(HttpCommandClientContext<>).MakeGenericType(commandType).GetConstructors().Single();

        var lambda = Expression.Lambda<TypeFactory>(
            Expression.New(constructor, pipelineIdParameter, envelopeParameter),
            pipelineIdParameter,
            envelopeParameter);

        return lambda.Compile();
    }
}
