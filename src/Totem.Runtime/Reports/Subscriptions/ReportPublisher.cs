using System.Collections.Concurrent;
using Totem.Map;

namespace Totem.Reports.Subscriptions;

internal class ReportPublisher
{
    readonly ReportBroker _broker;
    readonly ReportListSubscription _listSubscription;
    readonly ConcurrentDictionary<Id, ReportRouteSubscription> _routeSubscriptionsByRouteId;

    internal ReportPublisher(ReportBroker broker, ReportType report)
    {
        _broker = broker;
        _listSubscription = new(this);
        _routeSubscriptionsByRouteId = new();
        Report = report;
    }

    internal ReportType Report { get; }

    internal void Publish(ReportVersion version)
    {
        _listSubscription.Publish(version);

        if(_routeSubscriptionsByRouteId.TryGetValue(version.RouteId, out var routeSubscription))
        {
            routeSubscription.Publish(version);
        }
    }

    internal void Subscribe(ReportSubscriber subscriber, ReportETag etag)
    {
        subscriber.Subscribe(this);

        if(etag.IsList)
        {
            _listSubscription.Subscribe(subscriber, etag);
        }
        else
        {
            var routeSubscription = _routeSubscriptionsByRouteId.GetOrAdd(etag.RouteId, _ => new(this, etag));

            routeSubscription.Subscribe(subscriber, etag);
        }
    }

    internal void Unsubscribe(ReportSubscriber subscriber, ReportETag etag)
    {
        if(etag.IsList)
        {
            _listSubscription.Unsubscribe(subscriber);
        }
        else
        {
            if(_routeSubscriptionsByRouteId.TryGetValue(etag.RouteId, out var routeSubscription))
            {
                routeSubscription.Unsubscribe(subscriber);
            }
        }
    }

    internal void Unsubscribe(ReportSubscriber subscriber)
    {
        _listSubscription.Unsubscribe(subscriber);

        foreach(var routeSubscription in _routeSubscriptionsByRouteId.Values)
        {
            routeSubscription.Unsubscribe(subscriber);
        }
    }

    internal void EnqueueNotification(ReportNotification notification) =>
        _broker.EnqueueNotification(notification);

    internal void Remove(ReportRouteSubscription routeSubscription) =>
        _routeSubscriptionsByRouteId.Remove(routeSubscription.RouteId, out var _);
}
