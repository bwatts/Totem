using System.Collections.Generic;
using System.Threading.Tasks;

namespace Totem.Timeline.Client
{
  public class EmptyQueryNotifier : IQueryNotifier
  {
    public Task NotifyChanged(QueryETag etag, IEnumerable<Id> connectionIds)
    {
      return Task.CompletedTask;
    }

    public Task NotifyStopped(QueryETag etag, string error, IEnumerable<Id> connectionIds)
    {
      return Task.CompletedTask;
    }
  }
}