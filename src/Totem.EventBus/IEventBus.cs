using System.Collections.Generic;
using System.Threading.Tasks;

namespace Totem.EventBus
{
  public interface IEventBus
  {
    Task Publish(IntegrationEvent @event);

    Task Start(IEnumerable<SubscriptionInfo> subscriptions);
  }
}
