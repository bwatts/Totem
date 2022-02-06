using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Totem.Core;
using Totem.Map;
using Totem.Reports;

namespace Totem.InMemory;

public class InMemoryReportStore : IReportStore
{
    readonly ConcurrentDictionary<ItemKey, ReportHistory> _historiesByKey = new();
    readonly IServiceProvider _services;
    readonly IStorage _storage;

    public InMemoryReportStore(IServiceProvider services, IStorage storage)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public Task<IReportTransaction> StartTransactionAsync(IReportContext<IEvent> context, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        var report = (IReport) _services.GetRequiredService(context.ReportKey.DeclaredType);

        report.Id = context.ReportKey.Id;

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
        readonly string _reportId;

        internal ReportHistory(IReportTransaction transaction)
        {
            _reportType = transaction.Context.ReportType;
            _reportId = transaction.Context.ReportKey.Id.ToShortString();

            var version = transaction.Report.Version ?? 0;

            if(version != 0)
                throw new UnexpectedVersionException($"Expected report {_reportType}/{_reportId} to not exist, but found @{version}");
        }

        internal void Load(IReport report)
        {
            if(report.Version is not null)
                throw new InvalidOperationException($"Expected a report with no version, found {report}@{report.Version}");

            report.Version = _events.Count;

            foreach(var e in _events)
            {
                _reportType.CallGivenIfDefined(report, e);
            }
        }

        internal ReportHistory Update(IReportTransaction transaction)
        {
            var expectedVersion = _events.Count;

            if(transaction.Report.Version != expectedVersion)
                throw new UnexpectedVersionException($"Expected report {_reportType}/{_reportId}@{expectedVersion}, but received @{transaction.Report.Version}");

            _events.Add(transaction.Context.EventContext.Event);

            return this;
        }
    }
}
