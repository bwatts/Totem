namespace Totem.Workflows;

public interface IWorkflowStore
{
    Task<IWorkflowTransaction> StartTransactionAsync(IWorkflowContext<IEvent> context, CancellationToken cancellationToken);
    Task CommitAsync(IWorkflowTransaction transaction, CancellationToken cancellationToken);
    Task RollbackAsync(IWorkflowTransaction transaction, CancellationToken cancellationToken);
}
