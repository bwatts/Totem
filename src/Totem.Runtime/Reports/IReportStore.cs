namespace Totem.Reports;

public interface IReportStore
{
    Task<IReportTransaction> StartTransactionAsync(IReportContext<IEvent> context, CancellationToken cancellationToken);
    Task CommitAsync(IReportTransaction transaction, CancellationToken cancellationToken);
    Task RollbackAsync(IReportTransaction transaction, CancellationToken cancellationToken);
}
