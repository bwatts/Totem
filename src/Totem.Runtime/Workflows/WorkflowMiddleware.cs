using Microsoft.Extensions.DependencyInjection;

namespace Totem.Workflows;

public class WorkflowMiddleware : IWorkflowMiddleware
{
    readonly Func<IWorkflowContext<IEvent>, Func<Task>, CancellationToken, Task> _middleware;

    public WorkflowMiddleware(Func<IWorkflowContext<IEvent>, Func<Task>, CancellationToken, Task> middleware) =>
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

    public Task InvokeAsync(IWorkflowContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken) =>
        _middleware(context, next, cancellationToken);
}

public class WorkflowMiddleware<TService> : IWorkflowMiddleware
    where TService : IWorkflowMiddleware
{
    readonly IServiceProvider _services;

    public WorkflowMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task InvokeAsync(IWorkflowContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken) =>
        _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
}
