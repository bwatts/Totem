namespace Totem.Reports;

public class ReportWhenMiddleware : IReportMiddleware
{
    readonly IReportStore _store;

    public ReportWhenMiddleware(IReportStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public async Task InvokeAsync(IReportContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        var transaction = await _store.StartTransactionAsync(context, cancellationToken);

        context.ReportType.CallWhenIfDefined(transaction.Report, context.EventContext);

        if(!context.EventContext.HasErrors)
        {
            await transaction.CommitAsync();
            await next();
        }
    }
}
