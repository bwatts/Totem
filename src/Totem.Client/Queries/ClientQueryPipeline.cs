using Microsoft.Extensions.Logging;
using Totem.Http;

namespace Totem.Queries;

public class ClientQueryPipeline : IClientQueryPipeline
{
    readonly ILogger _logger;
    readonly IReadOnlyList<IClientQueryMiddleware> _steps;
    readonly IClientQueryContextFactory _contextFactory;

    public ClientQueryPipeline(
        Id id,
        ILogger<ClientQueryPipeline> logger,
        IReadOnlyList<IClientQueryMiddleware> steps,
        IClientQueryContextFactory contextFactory)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id Id { get; }

    public async Task<IClientQueryContext<IHttpQuery>> RunAsync(IHttpQueryEnvelope envelope, CancellationToken cancellationToken)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        _logger.LogTrace("[query] Run client pipeline {@PipelineId} for {@QueryType}.{@QueryId}", Id, envelope.MessageKey.DeclaredType, envelope.MessageKey.Id);

        var context = _contextFactory.Create(Id, envelope);

        await RunStepAsync(0);

        return context;

        async Task RunStepAsync(int index)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                _logger.LogTrace("[query] Client pipeline {@PipelineId} cancelled", Id);
                return;
            }

            if(index >= _steps.Count)
            {
                _logger.LogTrace("[query] Client pipeline {@PipelineId} complete", Id);
                return;
            }

            await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
        }
    }
}
