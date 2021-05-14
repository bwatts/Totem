using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Reports;
using Totem.Routes;

namespace Totem.Workflows
{
    public class ReportMiddleware : IRouteMiddleware
    {
        readonly IReportService _service;

        public ReportMiddleware(IReportService service) =>
            _service = service ?? throw new ArgumentNullException(nameof(service));

        public async Task InvokeAsync(IRouteContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken)
        {
            if(next == null)
                throw new ArgumentNullException(nameof(next));

            await _service.HandleAsync(context, cancellationToken);

            await next();
        }
    }
}