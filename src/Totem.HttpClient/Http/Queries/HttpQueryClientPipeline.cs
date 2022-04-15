namespace Totem.Http.Queries;

public class HttpQueryClientPipeline : IHttpQueryClientPipeline
{
    readonly ILogger _logger;
    readonly IReadOnlyList<IHttpQueryClientMiddleware> _steps;
    readonly IHttpQueryClientContextFactory _contextFactory;

    public HttpQueryClientPipeline(
        Id id,
        ILogger<HttpQueryClientPipeline> logger,
        IReadOnlyList<IHttpQueryClientMiddleware> steps,
        IHttpQueryClientContextFactory contextFactory)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id Id { get; }

    public async Task<IHttpQueryClientContext<IHttpQuery>> RunAsync(IHttpQueryEnvelope envelope, CancellationToken cancellationToken)
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
