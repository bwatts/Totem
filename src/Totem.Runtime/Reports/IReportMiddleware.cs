using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Reports;

public interface IReportMiddleware
{
    Task InvokeAsync(IReportContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken);
}
