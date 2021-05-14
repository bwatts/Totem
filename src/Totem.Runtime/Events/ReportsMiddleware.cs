using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Totem.Reports;
using Totem.Routes;

namespace Totem.Events
{
    public class ReportsMiddleware : IEventMiddleware
    {
        readonly ConcurrentDictionary<Id, IRouteSubscription> _subscriptionsByRouteId = new();
        readonly IReportService _service;

        public ReportsMiddleware(IReportService service) =>
            _service = service ?? throw new ArgumentNullException(nameof(service));

        public async Task InvokeAsync(IEventContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(next == null)
                throw new ArgumentNullException(nameof(next));

            await Task.WhenAll(
                from subscriber in _service.Route(context.Event)
                let subscription = _subscriptionsByRouteId.GetOrAdd(subscriber.Address.RouteId, _ => subscriber.Subscribe())
                select subscription.EnqueueAsync(context.Envelope, cancellationToken));

            await next();
        }
    }
}