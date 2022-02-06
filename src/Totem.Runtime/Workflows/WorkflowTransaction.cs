namespace Totem.Workflows;

public class WorkflowTransaction : IWorkflowTransaction
{
    readonly IWorkflowStore _store;
    readonly CancellationToken _cancellationToken;

    public WorkflowTransaction(IWorkflowStore store, IWorkflowContext<IEvent> context, IWorkflow workflow, CancellationToken cancellationToken)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
        _cancellationToken = cancellationToken;
    }

    public IWorkflowContext<IEvent> Context { get; }
    public IWorkflow Workflow { get; }

    public async Task CommitAsync()
    {
        if(!_cancellationToken.IsCancellationRequested)
        {
            await _store.CommitAsync(this, _cancellationToken);
        }
    }

    public Task RollbackAsync() =>
        _store.RollbackAsync(this, _cancellationToken);
}
