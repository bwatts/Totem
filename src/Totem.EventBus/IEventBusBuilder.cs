using System;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.EventBus
{
  public interface IEventBusBuilder
  {
    IEventBusBuilder ConfigureServices(Action<IServiceCollection> configure);
    void Subscribe<T, TH>(string eventName) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    void Unsubscribe<T, TH>(string eventName) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
  }
}