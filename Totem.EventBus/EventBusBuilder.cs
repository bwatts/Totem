namespace Totem.EventBus
{
  public class EventBusBuilder: IEventBusBuilder
  {
    private readonly IEventBus _eventBus;

    public EventBusBuilder(IEventBus eventBus)
    {
      _eventBus = eventBus;
    }

    public void Subscribe<T, TH>(string eventName)
      where T : IntegrationEvent
      where TH : IIntegrationEventHandler<T>
    {
      _eventBus.Subscribe<T, TH>(eventName);
    }

    public void Unsubscribe<T, TH>(string eventName)
      where T : IntegrationEvent
      where TH : IIntegrationEventHandler<T>
    {
      _eventBus.Unsubscribe<T, TH>(eventName);
    }
  }
}
