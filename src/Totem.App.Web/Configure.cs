using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Totem.Timeline.EventStore.Hosting;
using Totem.Timeline.Hosting;

namespace Totem.App.Web
{
  /// <summary>
  /// Configures the hosting of a Totem service application
  /// </summary>
  public static class Configure
  {
    public static ConfigureWebApp Host(Action<IWebHostBuilder> configure) =>
      new ConfigureWebApp().Host(configure);

    public static ConfigureWebApp App(Action<IApplicationBuilder> configure) =>
      new ConfigureWebApp().App(configure);

    public static ConfigureWebApp AppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configure) =>
      new ConfigureWebApp().AppConfiguration(configure);

    public static ConfigureWebApp Services(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      new ConfigureWebApp().Services(configure);

    public static ConfigureWebApp Serilog(Action<WebHostBuilderContext, LoggerConfiguration> configure) =>
      new ConfigureWebApp().Serilog(configure);

    public static ConfigureWebApp Timeline(Action<WebHostBuilderContext, ITimelineClientBuilder> configure) =>
      new ConfigureWebApp().Timeline(configure);

    public static ConfigureWebApp EventStore(Action<WebHostBuilderContext, IEventStoreTimelineClientBuilder> configure) =>
      new ConfigureWebApp().EventStore(configure);

    public static ConfigureWebApp Mvc(Action<WebHostBuilderContext, IMvcBuilder> configure) =>
      new ConfigureWebApp().Mvc(configure);

    public static ConfigureWebApp SignalR(Action<WebHostBuilderContext, ISignalRServerBuilder> configure) =>
      new ConfigureWebApp().SignalR(configure);

    public static ConfigureWebApp MvcRoutes(Action<IRouteBuilder> configure) =>
      new ConfigureWebApp().MvcRoutes(configure);

    public static ConfigureWebApp SignalRRoutes(Action<HubRouteBuilder> configure) =>
      new ConfigureWebApp().SignalRRoutes(configure);

    public static ConfigureWebApp AppConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureWebApp().AppConfiguration(configure);

    public static ConfigureWebApp Services(Action<IServiceCollection> configure) =>
      new ConfigureWebApp().Services(configure);

    public static ConfigureWebApp Timeline(Action<ITimelineClientBuilder> configure) =>
      new ConfigureWebApp().Timeline(configure);

    public static ConfigureWebApp EventStore(Action<IEventStoreTimelineClientBuilder> configure) =>
      new ConfigureWebApp().EventStore(configure);

    public static ConfigureWebApp Serilog(Action<LoggerConfiguration> configure) =>
      new ConfigureWebApp().Serilog(configure);

    public static ConfigureWebApp Mvc(Action<IMvcBuilder> configure) =>
      new ConfigureWebApp().Mvc(configure);

    public static ConfigureWebApp SignalR(Action<ISignalRServerBuilder> configure) =>
      new ConfigureWebApp().SignalR(configure);
  }
}