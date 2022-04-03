using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Totem.Reports.Subscriptions;

namespace Totem.Subscriptions;

public sealed class SubscriptionHubReportChannel : IReportChannel, IDisposable
{
    readonly Channel<ReportNotification> _channel = Channel.CreateUnbounded<ReportNotification>(new UnboundedChannelOptions
    {
        SingleReader = true
    });
    readonly CancellationTokenSource _cancellation = new();
    readonly ILogger _logger;
    readonly IHubContext<SubscriptionHub> _hubContext;

    public SubscriptionHubReportChannel(ILogger<SubscriptionHubReportChannel> logger, IHubContext<SubscriptionHub> hubContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

        Task.Run(SendNotificationsAsync);
    }

    public void EnqueueNotification(ReportNotification notification)
    {
        _logger.LogTrace("[subscriptions] Enqueue notification for report {ETag}", notification.ETag);

        _channel.Writer.TryWrite(notification);
    }

    public void Dispose()
    {
        _channel.Writer.Complete();
        _cancellation.Cancel();
    }

    async Task SendNotificationsAsync()
    {
        try
        {
            while(await _channel.Reader.WaitToReadAsync(_cancellation.Token))
            {
                if(_channel.Reader.TryRead(out var notification))
                {
                    var connectionIds = notification.SubscriberIds.Select(x => x.ToString());
                    var clients = _hubContext.Clients.Clients(connectionIds);

                    await clients.SendAsync("onReportPublished", notification.ETag.ToString());
                }

                _logger.LogTrace("[subscriptions] Wait for next report notification...");
            }
        }
        catch(OperationCanceledException)
        { }
        catch(ChannelClosedException)
        { }
        catch(Exception exception)
        {
            _logger.LogError(exception, "[subscriptions] Unhandled exception; subscription closed");
        }
    }
}
