namespace Totem.Reports.Subscriptions;

public interface IReportChannel
{
    void EnqueueNotification(ReportNotification notification);
}
