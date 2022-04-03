namespace Totem.Reports.Subscriptions;

public interface IReportBroker
{
    void Publish(ReportVersion etag);
    void Subscribe(Id subscriberId, ReportETag etag);
    void Unsubscribe(Id subscriberId, ReportETag etag);
    void Unsubscribe(Id subscriberId);
}
