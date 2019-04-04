using System;
using System.ComponentModel;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime.Hosting;
using Totem.Runtime.Json;
using Totem.Timeline.Area;
using Totem.Timeline.Hosting;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore.Hosting
{
  /// <summary>
  /// Extends <see cref="ITimelineBuilder"/> to declare the EventStore timeline database
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class EventStoreServiceExtensions
  {
    public static IEventStoreTimelineBuilder AddEventStore(this ITimelineBuilder timeline)
    {
      timeline.ConfigureServices(services =>
      {
        services.AddSingleton<ILogger, EventStoreLogAdapter>();

        services.AddSingleton<ITimelineDb>(p => new TimelineDb(
          p.GetRequiredService<EventStoreContext>(),
          p.BuildSubscriptionSettings(),
          p.GetRequiredService<IResumeProjection>()));

        services.AddSingleton(p => new EventStoreContext(
          p.BuildConnection(),
          p.GetRequiredService<IJsonFormat>(),
          p.GetRequiredService<AreaMap>()));

        services.AddSingleton<IResumeProjection>(p => new ResumeProjection(
          p.GetRequiredService<AreaMap>(),
          p.GetRequiredService<ProjectionsManager>(),
          p.BuildProjectionsCredentials()));

        services.AddSingleton(BuildProjectionsManager);
      });

      return new EventStoreTimelineBuilder(timeline);
    }

    public static IEventStoreTimelineBuilder BindOptionsToConfiguration(this IEventStoreTimelineBuilder timeline, string key = "totem.timeline.eventStore") =>
      timeline.ConfigureServices(services => services.BindOptionsToConfiguration<EventStoreTimelineOptions>(key));

    class EventStoreTimelineBuilder : IEventStoreTimelineBuilder
    {
      readonly ITimelineBuilder _timeline;

      internal EventStoreTimelineBuilder(ITimelineBuilder timeline)
      {
        _timeline = timeline;
      }

      public IEventStoreTimelineBuilder ConfigureServices(Action<IServiceCollection> configure)
      {
        _timeline.ConfigureServices(configure);

        return this;
      }
    }

    public static IEventStoreConnection BuildConnection(this IServiceProvider provider)
    {
      var options = provider.GetOptions<EventStoreTimelineOptions>();
      var settings = ConnectionSettings.Create();

      if(options.Verbose)
      {
        settings.EnableVerboseLogging();
      }

      settings.UseCustomLogger(provider.GetRequiredService<ILogger>());

      var connection = options.Connection;
      var reconnects = options.Reconnects;
      var heartbeat = options.Heartbeat;
      var operations = options.Operations;

      settings.WithConnectionTimeoutOf(connection.Timeout);

      settings.SetDefaultUserCredentials(new UserCredentials(
        connection.Username,
        connection.Password));

      settings.SetReconnectionDelayTo(reconnects.Delay);

      if(reconnects.Limit > 0)
      {
        settings.LimitReconnectionsTo(reconnects.Limit);
      }
      else
      {
        settings.KeepReconnecting();
      }

      settings.SetHeartbeatInterval(heartbeat.Interval);
      settings.SetHeartbeatTimeout(heartbeat.Timeout);

      settings.LimitAttemptsForOperationTo(operations.AttemptLimit);
      settings.LimitOperationsQueueTo(operations.QueueLimit);
      settings.SetOperationTimeoutTo(operations.Timeout);
      settings.SetTimeoutCheckPeriodTo(operations.TimeoutCheckPeriod);

      if(operations.FailOnNoServerResponse)
      {
        settings.FailOnNoServerResponse();
      }

      if(operations.RetryLimit > 0)
      {
        settings.LimitRetriesForOperationTo(operations.RetryLimit);
      }
      else
      {
        settings.KeepRetrying();
      }

      var uri = new Uri($"tcp://{options.Server.Name}:{options.Server.TcpPort}/");

      return EventStoreConnection.Create(settings.Build(), uri);
    }

    static CatchUpSubscriptionSettings BuildSubscriptionSettings(this IServiceProvider provider)
    {
      var options = provider.GetOptions<EventStoreTimelineOptions>();

      return new CatchUpSubscriptionSettings(
        options.Subscription.MaxLiveQueueSize,
        options.Subscription.ReadBatchSize,
        options.Verbose,
        resolveLinkTos: false);
    }

    static UserCredentials BuildProjectionsCredentials(this IServiceProvider provider)
    {
      var options = provider.GetOptions<EventStoreTimelineOptions>();

      return new UserCredentials(options.Connection.Username, options.Connection.Password);
    }

    static ProjectionsManager BuildProjectionsManager(this IServiceProvider provider)
    {
      var options = provider.GetOptions<EventStoreTimelineOptions>();

      return new ProjectionsManager(
        provider.GetRequiredService<ILogger>(),
        new DnsEndPoint(options.Server.Name, options.Server.HttpPort),
        options.Projections.InstallTimeout);
    }
  }
}