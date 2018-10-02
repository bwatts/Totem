using System.Threading.Tasks;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Describes a database containing the state of an area's queries
  /// </summary>
  public interface IQueryDb
  {
    Task<QueryState> ReadState(QueryETag etag);

    Task SubscribeToChanged(Id connectionId, QueryETag queryETag);

    void UnsubscribeFromChanged(Id connectionId, FlowKey queryKey);

    void UnsubscribeFromChanged(Id connectionId);
  }
}