using Totem.Events;
using Totem.Map;

namespace Totem.Reports;

public class ReportEventMiddleware : IEventMiddleware
{
    readonly RuntimeMap _map;
    readonly IReportPipeline _pipeline;

    public ReportEventMiddleware(RuntimeMap map, IReportPipeline pipeline)
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
            from reportKey in _map.CallReportRoutes(context)
            select _pipeline.RunAsync(context, reportKey, cancellationToken));

        await next();
    }
}
