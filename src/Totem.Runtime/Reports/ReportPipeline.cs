using Microsoft.Extensions.Logging;
using Totem.Core;

namespace Totem.Reports;

public class ReportPipeline : IReportPipeline
{
    readonly ILogger _logger;
    readonly IReadOnlyList<IReportMiddleware> _steps;
    readonly IReportContextFactory _contextFactory;

    public ReportPipeline(
        Id id,
        ILogger<ReportPipeline> logger,
        IReadOnlyList<IReportMiddleware> steps,
        IReportContextFactory contextFactory)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steps = steps ?? throw new ArgumentNullException(nameof(steps));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id Id { get; }

    public async Task<IReportContext<IEvent>> RunAsync(IEventContext<IEvent> eventContext, ItemKey reportKey, CancellationToken cancellationToken)
    {
        if(eventContext is null)
            throw new ArgumentNullException(nameof(eventContext));

        if(reportKey is null)
            throw new ArgumentNullException(nameof(reportKey));

        var envelope = eventContext.Envelope;

        _logger.LogTrace(
            "[report] Run pipeline {@PipelineId} for report {@ReportType}.{@ReportId} and event {@EventType}.{@EventId}",
            Id,
            reportKey.DeclaredType,
            reportKey.Id,
            envelope.MessageKey.DeclaredType,
            envelope.MessageKey.Id);

        var context = _contextFactory.Create(Id, eventContext, reportKey);

        await RunStepAsync(0);

        return context;

        async Task RunStepAsync(int index)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                _logger.LogTrace("[report] Pipeline {@PipelineId} for report {@ReportType}.{@ReportId} cancelled", Id, reportKey.DeclaredType, reportKey.Id);
                return;
            }

            if(index >= _steps.Count)
            {
                _logger.LogTrace("[report] Pipeline {@PipelineId} for report {@ReportType}.{@ReportId} complete", Id, reportKey.DeclaredType, reportKey.Id);
                return;
            }

            await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
        }
    }
}
