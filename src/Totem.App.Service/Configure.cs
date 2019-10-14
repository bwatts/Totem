using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Totem.Timeline.Hosting;

namespace Totem.App.Service
{
  /// <summary>
  /// Configures the hosting of a Totem service application
  /// </summary>
  public static class Configure
  {
    public static ConfigureServiceApp DisableSerilog() =>
      new ConfigureServiceApp().DisableSerilog();

    public static ConfigureServiceApp BeforeHost(Action<IHostBuilder> configure) =>
      new ConfigureServiceApp().BeforeHost(configure);

    public static ConfigureServiceApp BeforeHostConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().BeforeHostConfiguration(configure);

    public static ConfigureServiceApp BeforeAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().BeforeAppConfiguration(configure);

    public static ConfigureServiceApp BeforeServices(Action<HostBuilderContext, IServiceCollection> configure) =>
      new ConfigureServiceApp().BeforeServices(configure);

    public static ConfigureServiceApp BeforeTimeline(Action<HostBuilderContext, ITimelineBuilder> configure) =>
      new ConfigureServiceApp().BeforeTimeline(configure);

    public static ConfigureServiceApp BeforeSerilog(Action<HostBuilderContext, LoggerConfiguration> configure) =>
      new ConfigureServiceApp().BeforeSerilog(configure);

    public static ConfigureServiceApp AfterHost(Action<IHostBuilder> configure) =>
      new ConfigureServiceApp().AfterHost(configure);

    public static ConfigureServiceApp AfterHostConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().AfterHostConfiguration(configure);

    public static ConfigureServiceApp AfterAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().AfterAppConfiguration(configure);

    public static ConfigureServiceApp AfterServices(Action<HostBuilderContext, IServiceCollection> configure) =>
      new ConfigureServiceApp().AfterServices(configure);

    public static ConfigureServiceApp AfterTimeline(Action<HostBuilderContext, ITimelineBuilder> configure) =>
      new ConfigureServiceApp().AfterTimeline(configure);

    public static ConfigureServiceApp AfterSerilog(Action<HostBuilderContext, LoggerConfiguration> configure) =>
      new ConfigureServiceApp().AfterSerilog(configure);

    public static ConfigureServiceApp ReplaceHost(Action<IHostBuilder> configure) =>
      new ConfigureServiceApp().ReplaceHost(configure);

    public static ConfigureServiceApp ReplaceHostConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().ReplaceHostConfiguration(configure);

    public static ConfigureServiceApp ReplaceAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().ReplaceAppConfiguration(configure);

    public static ConfigureServiceApp ReplaceServices(Action<HostBuilderContext, IServiceCollection> configure) =>
      new ConfigureServiceApp().ReplaceServices(configure);

    public static ConfigureServiceApp ReplaceTimeline(Action<HostBuilderContext, ITimelineBuilder> configure) =>
      new ConfigureServiceApp().ReplaceTimeline(configure);

    public static ConfigureServiceApp ReplaceSerilog(Action<HostBuilderContext, LoggerConfiguration> configure) =>
      new ConfigureServiceApp().ReplaceSerilog(configure);

    //
    // Without HostBuilderContext
    //

    public static ConfigureServiceApp BeforeAppConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().BeforeAppConfiguration(configure);

    public static ConfigureServiceApp BeforeServices(Action<IServiceCollection> configure) =>
      new ConfigureServiceApp().BeforeServices(configure);

    public static ConfigureServiceApp BeforeTimeline(Action<ITimelineBuilder> configure) =>
      new ConfigureServiceApp().BeforeTimeline(configure);

    public static ConfigureServiceApp BeforeSerilog(Action<LoggerConfiguration> configure) =>
      new ConfigureServiceApp().BeforeSerilog(configure);

    public static ConfigureServiceApp AfterAppConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().AfterAppConfiguration(configure);

    public static ConfigureServiceApp AfterServices(Action<IServiceCollection> configure) =>
      new ConfigureServiceApp().AfterServices(configure);

    public static ConfigureServiceApp AfterTimeline(Action<ITimelineBuilder> configure) =>
      new ConfigureServiceApp().AfterTimeline(configure);

    public static ConfigureServiceApp AfterSerilog(Action<LoggerConfiguration> configure) =>
      new ConfigureServiceApp().AfterSerilog(configure);

    public static ConfigureServiceApp ReplaceAppConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureServiceApp().ReplaceAppConfiguration(configure);

    public static ConfigureServiceApp ReplaceServices(Action<IServiceCollection> configure) =>
      new ConfigureServiceApp().ReplaceServices(configure);

    public static ConfigureServiceApp ReplaceTimeline(Action<ITimelineBuilder> configure) =>
      new ConfigureServiceApp().ReplaceTimeline(configure);

    public static ConfigureServiceApp ReplaceSerilog(Action<LoggerConfiguration> configure) =>
      new ConfigureServiceApp().ReplaceSerilog(configure);
  }
}