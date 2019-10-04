using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Totem.Timeline.Area;
using Totem.Timeline.Client;

namespace Totem.Timeline.SignalR
{
  /// <summary>
	/// Communicates with clients subscribed to changed query notifications
	/// </summary>
  public class QueryHub : Hub
  {
    readonly AreaMap _area;
    readonly IQueryHost _host;

    public QueryHub(AreaMap area, IQueryHost host)
    {
      _area = area;
      _host = host;
    }

    Id ConnectionId => Id.From(Context.ConnectionId);

    public Task SubscribeToChanged(string etag) =>
      _host.SubscribeToChanged(ConnectionId, QueryETag.From(etag, _area));

    public void UnsubscribeFromChanged(string key) =>
      _host.UnsubscribeFromChanged(ConnectionId, FlowKey.From(key, _area));

    public override Task OnDisconnectedAsync(Exception exception)
    {
      _host.UnsubscribeFromChanged(ConnectionId);

      return base.OnDisconnectedAsync(exception);
    }
  }
}