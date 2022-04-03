using System.Collections.Concurrent;
using Totem.Map;

namespace Totem.Reports.Subscriptions;

internal class ReportListSubscription
{
    readonly ConcurrentDictionary<Id, bool> _subscriberIds = new();
    readonly ReportPublisher _publisher;
    ReportETag? _etag;

    internal ReportListSubscription(ReportPublisher publisher) =>
        _publisher = publisher;

    internal ReportType Report => _publisher.Report;

    internal void Publish(ReportVersion version)
    {
        lock(_subscriberIds)
        {
            if(_etag is not null)
            {
                _publisher.EnqueueNotification(new(_etag, _subscriberIds.Keys.ToList()));
            }

            _etag = version.ToListETag();
        }
    }

    internal void Subscribe(ReportSubscriber subscriber, ReportETag etag)
    {
        lock(_subscriberIds)
        {
            _subscriberIds[subscriber.Id] = true;

            if(etag != _etag)
            {
                _publisher.EnqueueNotification(new(etag, subscriber.Id));
            }
        }
    }

    internal void Unsubscribe(ReportSubscriber subscriber) =>
        _subscriberIds.TryRemove(subscriber.Id, out var _);
}
