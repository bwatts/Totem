namespace Totem.Commands;

public interface IClientCommandMiddleware
{
    Task InvokeAsync(IClientCommandContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken);
}
