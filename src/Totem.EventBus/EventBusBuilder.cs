using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.EventBus
{
  public class EventBusBuilder : IEventBusBuilder
  {
    readonly IServiceCollection _services;

    internal List<SubscriptionInfo> Subscriptions { get; }

    internal EventBusBuilder(IServiceCollection services)
    {
      _services = services;
      Subscriptions = new List<SubscriptionInfo>();
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
      _services.AddSingleton(typeof(TH));
      Subscriptions.Add(new SubscriptionInfo(eventName, typeof(T), typeof(TH)));
    }

    public void Unsubscribe<T, TH>(string eventName)
      where T : IntegrationEvent
      where TH : IIntegrationEventHandler<T>
    {
      Subscriptions.RemoveAll(x => x.EventName == eventName && x.EventType == typeof(T) && x.HandlerType == typeof(TH));
    }
  }
}
