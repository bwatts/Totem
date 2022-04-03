using System.Collections.Concurrent;
using Totem.Map;

namespace Totem.Reports.Subscriptions;

public class ReportBroker : IReportBroker
{
    readonly ConcurrentDictionary<ReportType, ReportPublisher> _publishersByReport = new();
    readonly ConcurrentDictionary<Id, ReportSubscriber> _subscribersById = new();
    readonly IReportChannel _channel;

    public ReportBroker(IReportChannel channel) =>
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));

    public void Publish(ReportVersion version)
    {
        if(version is null)
            throw new ArgumentNullException(nameof(version));

        var publisher = _publishersByReport.GetOrAdd(version.Report, report => new(this, report));

        publisher.Publish(version);
    }

    public void Subscribe(Id subscriberId, ReportETag etag)
    {
        if(subscriberId is null)
            throw new ArgumentNullException(nameof(subscriberId));

        if(etag is null)
            throw new ArgumentNullException(nameof(etag));

        var subscriber = _subscribersById.GetOrAdd(subscriberId, _ => new(subscriberId));
        var publisher = _publishersByReport.GetOrAdd(etag.Report, report => new(this, report));

        publisher.Subscribe(subscriber, etag);
    }

    public void Unsubscribe(Id subscriberId, ReportETag etag)
    {
        if(subscriberId is null)
            throw new ArgumentNullException(nameof(subscriberId));

        if(etag is null)
            throw new ArgumentNullException(nameof(etag));

        if(_subscribersById.TryGetValue(subscriberId, out var subscriber))
        {
            subscriber.Unsubscribe(etag);
        }
    }

    public void Unsubscribe(Id subscriberId)
    {
        if(subscriberId is null)
            throw new ArgumentNullException(nameof(subscriberId));

        if(_subscribersById.TryGetValue(subscriberId, out var subscriber))
        {
            subscriber.Unsubscribe();
        }
    }

    internal void EnqueueNotification(ReportNotification notification) =>
        _channel.EnqueueNotification(notification);

    internal void Remove(ReportPublisher publisher) =>
        _publishersByReport.TryRemove(publisher.Report, out _);
}
