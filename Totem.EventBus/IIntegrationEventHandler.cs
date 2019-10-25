using System.Threading.Tasks;

namespace Totem.EventBus
{
  public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
      where TIntegrationEvent : IntegrationEvent
  {
    Task Handle(TIntegrationEvent @event);
  }

  public interface IIntegrationEventHandler
  {
  }
}
