using System.Threading.Tasks;

namespace Totem.Reports;

public interface IReportTransaction
{
    IReportContext<IEvent> Context { get; }
    IReport Report { get; }

    Task CommitAsync();
    Task RollbackAsync();
}
