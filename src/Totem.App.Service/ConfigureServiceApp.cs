using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Totem.Timeline.EventStore.Hosting;
using Totem.Timeline.Hosting;

namespace Totem.App.Service
{
  /// <summary>
  /// Configures the hosting of a Totem service application
  /// </summary>
  public class ConfigureServiceApp
  {
    Action<IHostBuilder> _host;
    Action<HostBuilderContext, IConfigurationBuilder> _appConfiguration;
    Action<HostBuilderContext, IServiceCollection> _services;
    Action<HostBuilderContext, ITimelineBuilder> _timeline;
    Action<HostBuilderContext, IEventStoreTimelineBuilder> _eventStore;
    Action<HostBuilderContext, LoggerConfiguration> _serilog;

    internal void ConfigureHost(IHostBuilder host) =>
      _host?.Invoke(host);

    internal void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder appConfiguration) =>
      _appConfiguration?.Invoke(context, appConfiguration);

    internal void ConfigureServices(HostBuilderContext context, IServiceCollection services) =>
      _services?.Invoke(context, services);

    internal void ConfigureTimeline(HostBuilderContext context, ITimelineBuilder timeline) =>
      _timeline?.Invoke(context, timeline);

    internal void ConfigureEventStore(HostBuilderContext context, IEventStoreTimelineBuilder eventStore) =>
      _eventStore?.Invoke(context, eventStore);

    internal void ConfigureSerilog(HostBuilderContext context, LoggerConfiguration serilog) =>
      _serilog?.Invoke(context, serilog);

    public ConfigureServiceApp Host(Action<IHostBuilder> configure)
    {
      _host = configure;

      return this;
    }

    public ConfigureServiceApp AppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure)
    {
      _appConfiguration = configure;

      return this;
    }

    public ConfigureServiceApp Services(Action<HostBuilderContext, IServiceCollection> configure)
    {
      _services = configure;

      return this;
    }

    public ConfigureServiceApp Timeline(Action<HostBuilderContext, ITimelineBuilder> configure)
    {
      _timeline = configure;

      return this;
    }

    public ConfigureServiceApp EventStore(Action<HostBuilderContext, IEventStoreTimelineBuilder> configure)
    {
      _eventStore = configure;

      return this;
    }

    public ConfigureServiceApp Serilog(Action<HostBuilderContext, LoggerConfiguration> configure)
    {
      _serilog = configure;

      return this;
    }

    public ConfigureServiceApp AppConfiguration(Action<IConfigurationBuilder> configure) =>
      AppConfiguration((host, appConfiguration) => configure(appConfiguration));

    public ConfigureServiceApp Services(Action<IServiceCollection> configure) =>
      Services((host, services) => configure(services));

    public ConfigureServiceApp Timeline(Action<ITimelineBuilder> configure) =>
      Timeline((host, timeline) => configure(timeline));

    public ConfigureServiceApp EventStore(Action<IEventStoreTimelineBuilder> configure) =>
      EventStore((host, eventStore) => configure(eventStore));

    public ConfigureServiceApp Serilog(Action<LoggerConfiguration> configure) =>
      Serilog((host, serilog) => configure(serilog));
  }
}