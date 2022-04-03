namespace Totem.Queues;

public class QueueCommandPipeline : IQueueCommandPipeline
{
    readonly ILogger _logger;
    readonly IReadOnlyList<IQueueCommandMiddleware> _steps;
    readonly IQueueCommandContextFactory _contextFactory;

    public QueueCommandPipeline(
        Id id,
        ILogger<QueueCommandPipeline> logger,
        IReadOnlyList<IQueueCommandMiddleware> steps,
        IQueueCommandContextFactory contextFactory)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id Id { get; }

    public async Task<IQueueCommandContext<IQueueCommand>> RunAsync(IQueueCommandEnvelope envelope, CancellationToken cancellationToken)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        _logger.LogTrace("[queue {QueueName}] Run pipeline {@PipelineId} for {@CommandType}.{@CommandId}", envelope.Info.QueueName, Id, envelope.MessageKey.DeclaredType, envelope.MessageKey.Id);

        var context = _contextFactory.Create(Id, envelope);

        await RunStepAsync(0);

        return context;

        async Task RunStepAsync(int index)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                _logger.LogTrace("[queue {QueueName}] Pipeline {@PipelineId} cancelled", envelope.Info.QueueName, Id);
                return;
            }

            if(index >= _steps.Count)
            {
                _logger.LogTrace("[queue {QueueName}] Pipeline {@PipelineId} complete", envelope.Info.QueueName, Id);
                return;
            }

            await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
        }
    }
}
