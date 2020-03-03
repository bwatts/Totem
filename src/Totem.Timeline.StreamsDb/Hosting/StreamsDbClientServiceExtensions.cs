using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime.Hosting;
using Totem.Runtime.Json;
using Totem.Timeline.Area;
using Totem.Timeline.Client;
using Totem.Timeline.StreamsDb.Client;
using Totem.Timeline.Hosting;

namespace Totem.Timeline.StreamsDb.Hosting
{
  /// <summary>
  /// Extends <see cref="ITimelineBuilder"/> to declare the EventStore timeline database
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class StreamsDbClientServiceExtensions
  {
    public static IStreamsDbTimelineClientBuilder AddStreamsDb(this ITimelineClientBuilder client, string connectionString, string areaName)
    {
      client.ConfigureServices(services =>
        services
        .AddSingleton(p => new StreamsDbContext(
          connectionString,
          areaName,
          p.GetRequiredService<IJsonFormat>(),
          p.GetRequiredService<AreaMap>()))
        .AddSingleton<IClientDb, ClientDb>());

      return new EventStoreTimelineClientBuilder(client);
    }

    public static IStreamsDbTimelineClientBuilder BindOptionsToConfiguration(this IStreamsDbTimelineClientBuilder client, string key = "totem.timeline.streamsdb") =>
      client.ConfigureServices(services =>
        services.BindOptionsToConfiguration<StreamsDbTimelineOptions>(key));

    class EventStoreTimelineClientBuilder : IStreamsDbTimelineClientBuilder
    {
      readonly ITimelineClientBuilder _client;

      internal EventStoreTimelineClientBuilder(ITimelineClientBuilder client)
      {
        _client = client;
      }

      public IStreamsDbTimelineClientBuilder ConfigureServices(Action<IServiceCollection> configure)
      {
        _client.ConfigureServices(configure);

        return this;
      }
    }
  }
}