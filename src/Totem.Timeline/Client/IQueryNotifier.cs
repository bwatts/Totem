using System.Collections.Generic;
using System.Threading.Tasks;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Describes the notification of connections interested in changed queries
  /// </summary>
  public interface IQueryNotifier
  {
    Task NotifyChanged(QueryETag etag, IEnumerable<Id> connectionIds);
  }
}