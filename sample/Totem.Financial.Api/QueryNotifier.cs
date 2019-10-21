using System.Collections.Generic;
using System.Threading.Tasks;
using Totem.Timeline.Client;

namespace Totem.Sample.Api
{
  public class QueryNotifier : IQueryNotifier
  {
    public Task NotifyChanged(QueryETag etag, IEnumerable<Id> connectionIds)
    {
      return Task.CompletedTask;
      // throw new NotImplementedException();
    }

    public Task NotifyStopped(QueryETag etag, string error, IEnumerable<Id> connectionIds)
    {
      return Task.CompletedTask;
      // throw new NotImplementedException();
    }
  }
}
