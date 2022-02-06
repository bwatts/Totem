namespace Totem.Local;

public class LocalClient : ILocalClient
{
    readonly ILocalCommandPipeline _commandPipeline;
    readonly ILocalQueryPipeline _queryPipeline;

    public LocalClient(ILocalCommandPipeline commandPipeline, ILocalQueryPipeline queryPipeline)
    {
        _commandPipeline = commandPipeline ?? throw new ArgumentNullException(nameof(commandPipeline));
        _queryPipeline = queryPipeline ?? throw new ArgumentNullException(nameof(queryPipeline));
    }

    public Task<ILocalCommandContext<ILocalCommand>> SendAsync(ILocalCommandEnvelope envelope, CancellationToken cancellationToken) =>
        _commandPipeline.RunAsync(envelope, cancellationToken);

    public Task<ILocalQueryContext<ILocalQuery>> SendAsync(ILocalQueryEnvelope envelope, CancellationToken cancellationToken) =>
        _queryPipeline.RunAsync(envelope, cancellationToken);
}
