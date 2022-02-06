namespace Totem.Workflows;

public interface IWorkflowPipelineBuilder
{
    Id PipelineId { get; set; }

    IWorkflowPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IWorkflowMiddleware;

    IWorkflowPipeline Build();
}
