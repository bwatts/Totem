using Totem.Routes;

namespace Totem.Reports
{
    public interface IReportPipelineBuilder
    {
        Id PipelineId { get; set; }

        IReportPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IRouteMiddleware;

        IReportPipeline Build();
    }
}