using Totem.Core;

namespace Totem.Workflows;

public class WorkflowPipeline : IWorkflowPipeline
{
    readonly ILogger _logger;
    readonly IReadOnlyList<IWorkflowMiddleware> _steps;
    readonly IWorkflowContextFactory _contextFactory;

    public WorkflowPipeline(
        Id id,
        ILogger<WorkflowPipeline> logger,
        IReadOnlyList<IWorkflowMiddleware> steps,
        IWorkflowContextFactory contextFactory)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id Id { get; }

    public async Task<IWorkflowContext<IEvent>> RunAsync(IEventContext<IEvent> eventContext, ItemKey workflowKey, CancellationToken cancellationToken)
    {
        if(eventContext is null)
            throw new ArgumentNullException(nameof(eventContext));

        if(workflowKey is null)
            throw new ArgumentNullException(nameof(workflowKey));

        var envelope = eventContext.Envelope;

        _logger.LogTrace(
            "[workflow] Run pipeline {@PipelineId} for workflow {@WorkflowType}.{@WorkflowId} and event {@EventType}.{@EventId}",
            Id,
            workflowKey.DeclaredType,
            workflowKey.Id,
            envelope.MessageKey.Id,
            envelope.MessageKey.DeclaredType);

        var context = _contextFactory.Create(Id, eventContext, workflowKey);

        await RunStepAsync(0);

        return context;

        async Task RunStepAsync(int index)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                _logger.LogTrace("[workflow] Pipeline {@PipelineId} for workflow {@WorkflowType}.{@WorkflowId} cancelled", Id, workflowKey.DeclaredType, workflowKey.Id);
                return;
            }

            if(index >= _steps.Count)
            {
                _logger.LogTrace("[workflow] Pipeline {@PipelineId} for workflow {@WorkflowType}.{@WorkflowId} complete", Id, workflowKey.DeclaredType, workflowKey.Id);
                return;
            }

            await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
        }
    }
}
