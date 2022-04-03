using Microsoft.AspNetCore.SignalR;
using Totem.Map;
using Totem.Reports.Subscriptions;

namespace Totem.Subscriptions;

public class SubscriptionHub : Hub
{
    readonly IReportBroker _reportBroker;
    readonly RuntimeMap _map;

    public SubscriptionHub(IReportBroker reportBroker, RuntimeMap map)
    {
        _reportBroker = reportBroker ?? throw new ArgumentNullException(nameof(reportBroker));
        _map = map ?? throw new ArgumentNullException(nameof(map));
    }

    Id SubscriberId => Id.From(Context.ConnectionId);

    public void SubscribeToReport(string etag)
    {
        if(!ReportETag.TryFrom(etag, _map, out var parsedETag))
            throw new ArgumentException($"ETag does not refer to a report: {etag}");

        _reportBroker.Subscribe(SubscriberId, parsedETag);
    }

    public void UnsubscribeFromReport(string etag)
    {
        if(!ReportETag.TryFrom(etag, _map, out var parsedETag))
            throw new ArgumentException($"ETag does not refer to a report: {etag}");

        _reportBroker.Unsubscribe(SubscriberId, parsedETag);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _reportBroker.Unsubscribe(SubscriberId);

        return Task.CompletedTask;
    }
}
