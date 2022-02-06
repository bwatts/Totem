namespace Totem.Workflows;

public interface IWorkflowMiddleware
{
    Task InvokeAsync(IWorkflowContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken);
}
