namespace Totem.Queues;

public interface IQueueCommandMiddleware
{
    Task InvokeAsync(IQueueCommandContext<IQueueCommand> context, Func<Task> next, CancellationToken cancellationToken);
}
