namespace Totem.Reports;

public interface IReportPipelineBuilder
{
    Id PipelineId { get; set; }

    IReportPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IReportMiddleware;

    IReportPipeline Build();
}
