using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Totem.EventBus
{
  public static partial class EventBusServiceExtensions
  {
    public static IServiceCollection AddEventBus(this IServiceCollection services, Action<IEventBusBuilder> configure)
    {
      var eventBusBuilder = new EventBusBuilder(services);

      configure(eventBusBuilder);

      services.AddTransient<IHostedService, EventBusHost>(sp =>
      {
        return new EventBusHost
        (
          sp.GetRequiredService<IEventBusContext>(),
          sp.GetRequiredService<IEventBus>(),
          eventBusBuilder.Subscriptions
        );
      });

      return services;
    }
  }
}
