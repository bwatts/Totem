namespace Totem.Workflows;

public class WorkflowWhenMiddleware : IWorkflowMiddleware
{
    readonly IWorkflowStore _store;

    public WorkflowWhenMiddleware(IWorkflowStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public async Task InvokeAsync(IWorkflowContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        var transaction = await _store.StartTransactionAsync(context, cancellationToken);

        context.WorkflowType.CallWhenIfDefined(transaction.Workflow, context.EventContext);

        if(!context.EventContext.HasErrors)
        {
            await transaction.CommitAsync();
            await next();
        }
    }
}
