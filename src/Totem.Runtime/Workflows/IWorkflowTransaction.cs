namespace Totem.Workflows;

public interface IWorkflowTransaction
{
    IWorkflowContext<IEvent> Context { get; }
    IWorkflow Workflow { get; }

    Task CommitAsync();
    Task RollbackAsync();
}
