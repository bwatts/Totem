using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Totem.Timeline.Client;

namespace Totem.Timeline.SignalR
{
  /// <summary>
  /// Notifies subscribed <see cref="QueryHub"/> connections of changed queries
  /// </summary>
  public sealed class QueryNotifier : IQueryNotifier
  {
    readonly IHubContext<QueryHub> _hubContext;

    public QueryNotifier(IHubContext<QueryHub> hubContext)
    {
      _hubContext = hubContext;
    }

    public Task NotifyChanged(QueryETag etag, IEnumerable<Id> subscriberIds) =>
      _hubContext
      .Clients
      .Clients(subscriberIds.ToMany(id => id.ToString()))
      .SendAsync("onChanged", etag.ToString());
  }
}