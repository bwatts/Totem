using Microsoft.Extensions.Logging;

namespace Totem.Http;

public class HttpCommandPipeline : IHttpCommandPipeline
{
    readonly ILogger _logger;
    readonly IReadOnlyList<IHttpCommandMiddleware> _steps;
    readonly IHttpCommandContextFactory _contextFactory;

    public HttpCommandPipeline(
        Id id,
        ILogger<HttpCommandPipeline> logger,
        IReadOnlyList<IHttpCommandMiddleware> steps,
        IHttpCommandContextFactory contextFactory)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id Id { get; }

    public async Task<IHttpCommandContext<IHttpCommand>> RunAsync(IHttpCommandEnvelope envelope, CancellationToken cancellationToken)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        _logger.LogTrace("[command] Run pipeline {@PipelineId} for {@CommandType}.{@CommandId}", Id, envelope.MessageKey.DeclaredType, envelope.MessageKey.Id);

        var context = _contextFactory.Create(Id, envelope);

        await RunStepAsync(0);

        return context;

        async Task RunStepAsync(int index)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                _logger.LogTrace("[command] Pipeline {@PipelineId} cancelled", Id);
                return;
            }

            if(index >= _steps.Count)
            {
                _logger.LogTrace("[command] Pipeline {@PipelineId} complete", Id);
                return;
            }

            await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
        }
    }
}
