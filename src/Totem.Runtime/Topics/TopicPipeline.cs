using Totem.Core;

namespace Totem.Topics;

public class TopicPipeline : ITopicPipeline
{
    readonly ILogger _logger;
    readonly IReadOnlyList<ITopicMiddleware> _steps;
    readonly ITopicContextFactory _contextFactory;

    public TopicPipeline(
        Id id,
        ILogger<TopicPipeline> logger,
        IReadOnlyList<ITopicMiddleware> steps,
        ITopicContextFactory contextFactory)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id Id { get; }

    public async Task<ITopicContext<ICommandMessage>> RunAsync(ICommandContext<ICommandMessage> commandContext, ItemKey topicKey, CancellationToken cancellationToken)
    {
        if(commandContext is null)
            throw new ArgumentNullException(nameof(commandContext));

        if(topicKey is null)
            throw new ArgumentNullException(nameof(topicKey));

        _logger.LogTrace(
            "[report] Run pipeline {@PipelineId} for topic {@TopicType}.{@TopicId} and command {@CommandType}.{@CommandId}",
            Id,
            topicKey.DeclaredType,
            topicKey.Id,
            commandContext.CommandKey.DeclaredType,
            commandContext.CommandKey.Id);

        var context = _contextFactory.Create(Id, commandContext, topicKey);

        await RunStepAsync(0);

        return context;

        async Task RunStepAsync(int index)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                _logger.LogTrace("[report] Pipeline {@PipelineId} for report {@ReportType}.{@ReportId} cancelled", Id, topicKey.DeclaredType, topicKey.Id);
                return;
            }

            if(index >= _steps.Count)
            {
                _logger.LogTrace("[report] Pipeline {@PipelineId} for report {@ReportType}.{@ReportId} complete", Id, topicKey.DeclaredType, topicKey.Id);
                return;
            }

            await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
        }
    }
}
