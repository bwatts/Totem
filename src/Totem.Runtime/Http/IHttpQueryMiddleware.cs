namespace Totem.Http;

public interface IHttpQueryMiddleware
{
    Task InvokeAsync(IHttpQueryContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken);
}
