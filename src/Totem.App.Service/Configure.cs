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
  public static class Configure
  {
    public static ConfigureServiceApp Host(Action<IHostBuilder> configure) =>
      new ConfigureServiceApp().Host(configure);

    public static ConfigureServiceApp AppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().AppConfiguration(configure);

    public static ConfigureServiceApp Services(Action<HostBuilderContext, IServiceCollection> configure) =>
      new ConfigureServiceApp().Services(configure);

    public static ConfigureServiceApp Serilog(Action<HostBuilderContext, LoggerConfiguration> configure) =>
      new ConfigureServiceApp().Serilog(configure);

    public static ConfigureServiceApp Timeline(Action<HostBuilderContext, ITimelineBuilder> configure) =>
      new ConfigureServiceApp().Timeline(configure);

    public static ConfigureServiceApp EventStore(Action<HostBuilderContext, IEventStoreTimelineBuilder> configure) =>
      new ConfigureServiceApp().EventStore(configure);

    public static ConfigureServiceApp AppConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().AppConfiguration(configure);

    public static ConfigureServiceApp Services(Action<IServiceCollection> configure) =>
      new ConfigureServiceApp().Services(configure);

    public static ConfigureServiceApp Serilog(Action<LoggerConfiguration> configure) =>
      new ConfigureServiceApp().Serilog(configure);

    public static ConfigureServiceApp Timeline(Action<ITimelineBuilder> configure) =>
      new ConfigureServiceApp().Timeline(configure);

    public static ConfigureServiceApp EventStore(Action<IEventStoreTimelineBuilder> configure) =>
      new ConfigureServiceApp().EventStore(configure);
  }
}