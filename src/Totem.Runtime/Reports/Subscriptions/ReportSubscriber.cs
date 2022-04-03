using System.Collections.Concurrent;
using Totem.Map;

namespace Totem.Reports.Subscriptions;

internal class ReportSubscriber
{
    readonly ConcurrentDictionary<ReportType, ReportPublisher> _publishersByReport = new();

    internal ReportSubscriber(Id id) =>
        Id = id;

    internal Id Id { get; }

    internal void Subscribe(ReportPublisher publisher) =>
        _publishersByReport[publisher.Report] = publisher;

    internal void Unsubscribe(ReportETag etag)
    {
        if(_publishersByReport.TryGetValue(etag.Report, out var publisher))
        {
            publisher.Unsubscribe(this, etag);
        }
    }

    internal void Unsubscribe()
    {
        foreach(var publisher in _publishersByReport.Values)
        {
            publisher.Unsubscribe(this);
        }
    }
}
