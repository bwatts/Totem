using System;
using System.ComponentModel;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime.Hosting;
using Totem.Runtime.Json;
using Totem.Timeline.Area;
using Totem.Timeline.Client;
using Totem.Timeline.EventStore.Client;
using Totem.Timeline.Hosting;

namespace Totem.Timeline.EventStore.Hosting
{
  /// <summary>
  /// Extends <see cref="ITimelineBuilder"/> to declare the EventStore timeline database
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class EventStoreClientServiceExtensions
  {
    public static IEventStoreTimelineClientBuilder AddEventStore(this ITimelineClientBuilder client)
    {
      client.ConfigureServices(services =>
        services
        .AddSingleton<ILogger, EventStoreLogAdapter>()
        .AddSingleton(p => new EventStoreContext(
          p.BuildConnection(),
          p.GetRequiredService<IJsonFormat>(),
          p.GetRequiredService<AreaMap>()))
        .AddSingleton<IClientDb, ClientDb>());

      return new EventStoreTimelineClientBuilder(client);
    }

    public static IEventStoreTimelineClientBuilder BindOptionsToConfiguration(this IEventStoreTimelineClientBuilder client, string key = "totem.timeline.eventStore") =>
      client.ConfigureServices(services =>
        services.BindOptionsToConfiguration<EventStoreTimelineOptions>(key));

    class EventStoreTimelineClientBuilder : IEventStoreTimelineClientBuilder
    {
      readonly ITimelineClientBuilder _client;

      internal EventStoreTimelineClientBuilder(ITimelineClientBuilder client)
      {
        _client = client;
      }

      public IEventStoreTimelineClientBuilder ConfigureServices(Action<IServiceCollection> configure)
      {
        _client.ConfigureServices(configure);

        return this;
      }
    }
  }
}