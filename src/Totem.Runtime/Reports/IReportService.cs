using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Totem.Routes;

namespace Totem.Reports
{
    public interface IReportService
    {
        IEnumerable<IRouteSubscriber> Route(IEvent e);

        Task HandleAsync(IRouteContext<IEvent> context, CancellationToken cancellationToken);
    }
}