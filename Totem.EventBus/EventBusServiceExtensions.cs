using System;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.EventBus
{
  public static class EventBusServiceExtensions
  {
    public static IServiceCollection AddEventBus(this IServiceCollection services, Action<IEventBusBuilder> configure)
    {
      services.AddHostedService<EventBusHost>();
      configure(new EventBusBuilder(services));
      return services;
    }

    class EventBusBuilder : IEventBusBuilder
    {
      readonly IServiceCollection _services;

      internal EventBusBuilder(IServiceCollection services)
      {
        _services = services;
      }

      public IEventBusBuilder ConfigureServices(Action<IServiceCollection> configure)
      {
        configure(_services);
        return this;
      }

      public void Subscribe<T, TH>(string eventName)
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
      {
        throw new NotImplementedException();
      }

      public void Unsubscribe<T, TH>(string eventName)
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
      {
        throw new NotImplementedException();
      }
    }
  }
}
