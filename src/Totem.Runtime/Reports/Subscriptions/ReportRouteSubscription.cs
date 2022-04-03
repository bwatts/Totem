using System.Collections.Concurrent;
using Totem.Map;

namespace Totem.Reports.Subscriptions;

internal class ReportRouteSubscription
{
    readonly ConcurrentDictionary<Id, bool> _subscriberIds = new();
    readonly ReportPublisher _publisher;
    ReportETag _etag;

    internal ReportRouteSubscription(ReportPublisher publisher, ReportETag etag)
    {
        _publisher = publisher;
        _etag = etag;
    }

    internal ReportType Report => _etag.Report;
    internal Id RouteId => _etag.RouteId;

    internal void Publish(ReportVersion version)
    {
        lock(_subscriberIds)
        {
            _publisher.EnqueueNotification(new(_etag, _subscriberIds.Keys.ToList()));

            _etag = version.ToRouteETag();
        }
    }

    internal void Subscribe(ReportSubscriber subscriber, ReportETag etag)
    {
        lock(_subscriberIds)
        {
            _subscriberIds[subscriber.Id] = true;

            if(etag.Version != _etag.Version)
            {
                _publisher.EnqueueNotification(new(_etag, subscriber.Id));
            }
        }
    }

    internal void Unsubscribe(ReportSubscriber subscriber)
    {
        if(_subscriberIds.TryRemove(subscriber.Id, out var _) && _subscriberIds.IsEmpty)
        {
            _publisher.Remove(this);
        }
    }
}
