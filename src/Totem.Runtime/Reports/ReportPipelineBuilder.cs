using Microsoft.Extensions.Logging;

namespace Totem.Reports;

public class ReportPipelineBuilder : IReportPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly IReportContextFactory _contextFactory;
    readonly List<IReportMiddleware> _steps = new();

    public ReportPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        IReportContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public IReportPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IReportMiddleware
    {
        if(middleware is not null)
        {
            _steps.Add(middleware);
        }
        else
        {
            _steps.Add(new ReportMiddleware<TMiddleware>(_services));
        }

        return this;
    }

    public IReportPipeline Build() =>
        new ReportPipeline(PipelineId, _loggerFactory.CreateLogger<ReportPipeline>(), _steps, _contextFactory);
}
