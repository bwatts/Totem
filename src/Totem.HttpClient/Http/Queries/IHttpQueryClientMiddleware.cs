namespace Totem.Http.Queries;

public interface IHttpQueryClientMiddleware
{
    Task InvokeAsync(IHttpQueryClientContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken);
}
