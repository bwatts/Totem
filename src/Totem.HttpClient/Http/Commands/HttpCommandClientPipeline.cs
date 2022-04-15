namespace Totem.Http.Commands;

public class HttpCommandClientPipeline : IHttpCommandClientPipeline
{
    readonly ILogger _logger;
    readonly IReadOnlyList<IHttpCommandClientMiddleware> _steps;
    readonly IHttpCommandClientContextFactory _contextFactory;

    public HttpCommandClientPipeline(
        Id id,
        ILogger<HttpCommandClientPipeline> logger,
        IReadOnlyList<IHttpCommandClientMiddleware> steps,
        IHttpCommandClientContextFactory contextFactory)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id Id { get; }

    public async Task<IHttpCommandClientContext<IHttpCommand>> RunAsync(IHttpCommandEnvelope envelope, CancellationToken cancellationToken)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        _logger.LogTrace("[command] Run client pipeline {@PipelineId} for {@CommandType}.{@CommandId}", Id, envelope.MessageKey.DeclaredType, envelope.MessageKey.Id);

        var context = _contextFactory.Create(Id, envelope);

        await RunStepAsync(0);

        return context;

        async Task RunStepAsync(int index)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                _logger.LogTrace("[command] Client pipeline {@PipelineId} cancelled", Id);
                return;
            }

            if(index >= _steps.Count)
            {
                _logger.LogTrace("[command] Client pipeline {@PipelineId} complete", Id);
                return;
            }

            await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
        }
    }
}
