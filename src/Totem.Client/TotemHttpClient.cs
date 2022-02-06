using Totem.Commands;
using Totem.Http;
using Totem.Queries;

namespace Totem;

public class TotemHttpClient : ITotemHttpClient
{
    readonly IClientCommandPipeline _commandPipeline;
    readonly IClientQueryPipeline _queryPipeline;

    public TotemHttpClient(IClientCommandPipeline commandPipeline, IClientQueryPipeline queryPipeline)
    {
        _commandPipeline = commandPipeline ?? throw new ArgumentNullException(nameof(commandPipeline));
        _queryPipeline = queryPipeline ?? throw new ArgumentNullException(nameof(queryPipeline));
    }

    public Task<IClientCommandContext<IHttpCommand>> SendAsync(IHttpCommandEnvelope envelope, CancellationToken cancellationToken) =>
        _commandPipeline.RunAsync(envelope, cancellationToken);

    public Task<IClientQueryContext<IHttpQuery>> SendAsync(IHttpQueryEnvelope envelope, CancellationToken cancellationToken) =>
        _queryPipeline.RunAsync(envelope, cancellationToken);
}
