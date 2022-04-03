namespace Totem.Reports.Subscriptions;

public class ReportNotification
{
    public ReportNotification(ReportETag etag, IReadOnlyList<Id> subscriberIds)
    {
        ETag = etag ?? throw new ArgumentNullException(nameof(etag));
        SubscriberIds = subscriberIds ?? throw new ArgumentNullException(nameof(subscriberIds));
    }

    public ReportNotification(ReportETag etag, params Id[] subscriberIds)
    {
        ETag = etag ?? throw new ArgumentNullException(nameof(etag));
        SubscriberIds = subscriberIds ?? throw new ArgumentNullException(nameof(subscriberIds));
    }

    public ReportNotification(ReportETag etag, Id subscriberId)
    {
        ETag = etag ?? throw new ArgumentNullException(nameof(etag));
        SubscriberIds = subscriberId is not null ? new[] { subscriberId } : throw new ArgumentNullException(nameof(subscriberId));
    }

    public ReportETag ETag { get; }
    public IReadOnlyList<Id> SubscriberIds { get; }
}
