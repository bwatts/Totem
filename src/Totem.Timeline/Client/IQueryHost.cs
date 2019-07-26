using System.Threading.Tasks;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Describes the hosting of subscription queries in the timeline client
  /// </summary>
  public interface IQueryHost
  {
    Task SubscribeToChanged(Id connectionId, QueryETag etag);

    Task UnsubscribeFromChanged(Id connectionId, FlowKey key);

    Task UnsubscribeFromChanged(Id connectionId);
  }
}