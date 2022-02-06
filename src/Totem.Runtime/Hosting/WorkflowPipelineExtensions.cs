using Totem.Workflows;

namespace Totem.Hosting;

public static class WorkflowPipelineExtensions
{
    public static IWorkflowPipelineBuilder Use(this IWorkflowPipelineBuilder builder, Func<IWorkflowContext<IEvent>, Func<Task>, CancellationToken, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use(new WorkflowMiddleware(middleware));
    }

    public static IWorkflowPipelineBuilder Use(this IWorkflowPipelineBuilder builder, Func<IWorkflowContext<IEvent>, Func<Task>, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(middleware is null)
            throw new ArgumentNullException(nameof(middleware));

        return builder.Use((context, next, _) => middleware(context, next));
    }

    public static IWorkflowPipelineBuilder UseWhenCall(this IWorkflowPipelineBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use<WorkflowWhenMiddleware>();
    }
}
