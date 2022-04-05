using System.Collections.Concurrent;
using Totem.Core;
using Totem.Map;
using Totem.Reports;

namespace Totem.InMemory;

public class InMemoryReportStore : IReportStore
{
    readonly ConcurrentDictionary<ItemKey, ReportHistory> _historiesByKey = new();
    readonly IStorage _storage;

    public InMemoryReportStore(IStorage storage) =>
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));

    public Task<IReportTransaction> StartTransactionAsync(IReportContext<IEvent> context, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        var report = context.ReportType.Create(context.ReportKey.Id);

        report.Row.Id = report.Id!;

        if(_historiesByKey.TryGetValue(context.ReportKey, out var history))
        {
            history.Load(report);
        }

        context.ReportType.CallGivenIfDefined(report, context.EventContext.Event);

        return Task.FromResult<IReportTransaction>(new ReportTransaction(this, context, report, cancellationToken));
    }

    public async Task CommitAsync(IReportTransaction transaction, CancellationToken cancellationToken)
    {
        if(transaction is null)
            throw new ArgumentNullException(nameof(transaction));

        var key = transaction.Context.ReportKey;

        _historiesByKey.AddOrUpdate(key, _ => new(transaction), (_, history) => history.Update(transaction));

        var storageRow = StorageRow.From(
            key.DeclaredType.FullName!,
            key.Id.ToString(),
            transaction.Report);

        await _storage.PutAsync(storageRow, cancellationToken);
    }

    public Task RollbackAsync(IReportTransaction transaction, CancellationToken cancellationToken)
    {
        // Nothing to do when in memory
        return Task.CompletedTask;
    }

    class ReportHistory
    {
        readonly List<IEvent> _events = new();
        readonly ReportType _reportType;

        internal ReportHistory(IReportTransaction transaction) =>
            _reportType = transaction.Context.ReportType;

        internal void Load(IReport report)
        {
            foreach(var e in _events)
            {
                _reportType.CallGivenIfDefined(report, e);
            }
        }

        internal ReportHistory Update(IReportTransaction transaction)
        {
            _events.Add(transaction.Context.EventContext.Event);
            return this;
        }
    }
}
