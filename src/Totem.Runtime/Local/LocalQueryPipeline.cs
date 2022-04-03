namespace Totem.Local;

public class LocalQueryPipeline : ILocalQueryPipeline
{
    readonly ILogger _logger;
    readonly IReadOnlyList<ILocalQueryMiddleware> _steps;
    readonly ILocalQueryContextFactory _contextFactory;

    public LocalQueryPipeline(
        Id id,
        ILogger<LocalQueryPipeline> logger,
        IReadOnlyList<ILocalQueryMiddleware> steps,
        ILocalQueryContextFactory contextFactory)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id Id { get; }

    public async Task<ILocalQueryContext<ILocalQuery>> RunAsync(ILocalQueryEnvelope envelope, CancellationToken cancellationToken)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        _logger.LogTrace("[query] Run pipeline {@PipelineId} for {@QueryType}.{@QueryId}", Id, envelope.MessageKey.DeclaredType, envelope.MessageKey.Id);

        var context = _contextFactory.Create(Id, envelope);

        await RunStepAsync(0);

        return context;

        async Task RunStepAsync(int index)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                _logger.LogTrace("[query] Pipeline {@PipelineId} cancelled", Id);
                return;
            }

            if(index >= _steps.Count)
            {
                _logger.LogTrace("[query] Pipeline {@PipelineId} complete", Id);
                return;
            }

            await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
        }
    }
}
