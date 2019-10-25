using Microsoft.Extensions.DependencyInjection;
using StreamsDB.Driver;
using System;

namespace Totem.EventBus.StreamsDb
{
  public static class ServiceExtensions
  {
    public static void AddStreamsDbEventBus(this IServiceCollection services, Action<IEventBusBuilder> configure)
    {
      services.AddSingleton<IEventBus>(sp =>
      {
        var streamsDbClient = sp.GetRequiredService<StreamsDBClient>();
        var eventBus = new StreamsDbEventBus(streamsDbClient, type => (IIntegrationEventHandler)sp.GetRequiredService(type));
        configure(new EventBusBuilder(eventBus));
        eventBus.Start();
        return eventBus;
      });
    }

    public static void AddStreamsDbEventBus(this IServiceCollection services)
    {
      services.AddSingleton<IEventBus>(sp =>
      {
        var streamsDbClient = sp.GetRequiredService<StreamsDBClient>();
        var eventBus = new StreamsDbEventBus(streamsDbClient, type => (IIntegrationEventHandler)sp.GetRequiredService(type));
        // eventBus.Start();
        return eventBus;
      });
    }
  }
}
