using System.Collections.Generic;
using System.Threading.Tasks;

namespace Totem.EventBus
{
  public interface IEventBus
  {
    Task Publish(IntegrationEvent @event);

    //void Subscribe<T, TH>(string eventName)
    //        where T : IntegrationEvent
    //        where TH : IIntegrationEventHandler<T>;

    //void Unsubscribe<T, TH>(string eventName)
    //    where TH : IIntegrationEventHandler<T>
    //    where T : IntegrationEvent;

    void Start(IEnumerable<SubscriptionInfo> subscriptions);
  }
}
