namespace Totem.Http.Commands;

public interface IHttpCommandClientMiddleware
{
    Task InvokeAsync(IHttpCommandClientContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken);
}
