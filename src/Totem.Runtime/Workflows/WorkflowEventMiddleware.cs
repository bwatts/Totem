using Totem.Events;
using Totem.Map;

namespace Totem.Workflows;

public class WorkflowEventMiddleware : IEventMiddleware
{
    readonly RuntimeMap _map;
    readonly IWorkflowPipeline _pipeline;

    public WorkflowEventMiddleware(RuntimeMap map, IWorkflowPipeline pipeline)
    {
        _map = map ?? throw new ArgumentNullException(nameof(map));
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    public async Task InvokeAsync(IEventContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        await Task.WhenAll(
            from workflowKey in _map.CallWorkflowRoutes(context)
            select _pipeline.RunAsync(context, workflowKey, cancellationToken));

        await next();
    }
}
