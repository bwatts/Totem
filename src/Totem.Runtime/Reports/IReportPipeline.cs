using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Reports;

public interface IReportPipeline
{
    Id Id { get; }

    Task<IReportContext<IEvent>> RunAsync(IEventContext<IEvent> eventContext, ItemKey reportKey, CancellationToken cancellationToken);
}
