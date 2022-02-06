namespace Totem.Queries;

public interface IClientQueryMiddleware
{
    Task InvokeAsync(IClientQueryContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken);
}
