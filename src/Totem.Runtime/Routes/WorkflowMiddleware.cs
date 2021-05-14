using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Workflows;

namespace Totem.Routes
{
    public class WorkflowMiddleware : IRouteMiddleware
    {
        readonly IWorkflowService _service;

        public WorkflowMiddleware(IWorkflowService service) =>
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