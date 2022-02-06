using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Reports;

public class ReportTransaction : IReportTransaction
{
    readonly IReportStore _store;
    readonly CancellationToken _cancellationToken;

    public ReportTransaction(IReportStore store, IReportContext<IEvent> context, IReport report, CancellationToken cancellationToken)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Report = report ?? throw new ArgumentNullException(nameof(report));
        _cancellationToken = cancellationToken;
    }

    public IReportContext<IEvent> Context { get; }
    public IReport Report { get; }

    public async Task CommitAsync()
    {
        if(!_cancellationToken.IsCancellationRequested)
        {
            await _store.CommitAsync(this, _cancellationToken);
        }
    }

    public Task RollbackAsync() =>
        _store.RollbackAsync(this, _cancellationToken);
}
