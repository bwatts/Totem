using Totem.Routes;

namespace Totem.Workflows
{
    public interface IWorkflowPipelineBuilder
    {
        Id PipelineId { get; set; }

        IWorkflowPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IRouteMiddleware;

        IWorkflowPipeline Build();
    }
}