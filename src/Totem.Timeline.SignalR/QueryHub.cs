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
    readonly IQueryDb _db;

    public QueryHub(AreaMap area, IQueryDb db)
    {
      _area = area;
      _db = db;
    }

    Id ConnectionId => Id.From(Context.ConnectionId);

    public Task SubscribeToChanged(string etag) =>
      _db.SubscribeToChanged(ConnectionId, QueryETag.From(etag, _area));

    public void UnsubscribeFromChanged(string key) =>
      _db.UnsubscribeFromChanged(ConnectionId, FlowKey.From(key, _area));

    public override Task OnDisconnectedAsync(Exception exception)
    {
      _db.UnsubscribeFromChanged(ConnectionId);

      return base.OnDisconnectedAsync(exception);
    }
  }
}