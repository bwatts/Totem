namespace Totem.EventBus
{
  public interface IEventBusBuilder
  {
    void Subscribe<T, TH>(string eventName)
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;

    void Unsubscribe<T, TH>(string eventName)
        where TH : IIntegrationEventHandler<T>
        where T : IntegrationEvent;
  }
}
