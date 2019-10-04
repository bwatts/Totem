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
  /// Configures the hosting of a Totem web application
  /// </summary>
  public class ConfigureWebApp
  {
    Action<IWebHostBuilder> _host;
    Action<IApplicationBuilder> _app;
    Action<WebHostBuilderContext, IConfigurationBuilder> _appConfiguration;
    Action<WebHostBuilderContext, IServiceCollection> _services;
    Action<WebHostBuilderContext, ITimelineClientBuilder> _timeline;
    Action<WebHostBuilderContext, IEventStoreTimelineClientBuilder> _eventStore;
    Action<WebHostBuilderContext, LoggerConfiguration> _serilog;
    Action<WebHostBuilderContext, IMvcBuilder> _mvc;
    Action<WebHostBuilderContext, ISignalRServerBuilder> _signalR;
    Action<IRouteBuilder> _mvcRoutes;
    Action<HubRouteBuilder> _signalRRoutes;

    internal void ConfigureHost(IWebHostBuilder host) =>
      _host?.Invoke(host);

    internal void ConfigureApp(IApplicationBuilder app) =>
      _app?.Invoke(app);

    internal void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder appConfiguration) =>
      _appConfiguration?.Invoke(context, appConfiguration);

    internal void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
      _services?.Invoke(context, services);

    internal void ConfigureTimeline(WebHostBuilderContext context, ITimelineClientBuilder timeline) =>
      _timeline?.Invoke(context, timeline);

    internal void ConfigureEventStore(WebHostBuilderContext context, IEventStoreTimelineClientBuilder eventStore) =>
      _eventStore?.Invoke(context, eventStore);

    internal void ConfigureSerilog(WebHostBuilderContext context, LoggerConfiguration serilog) =>
      _serilog?.Invoke(context, serilog);

    internal void ConfigureMvc(WebHostBuilderContext context, IMvcBuilder mvc) =>
      _mvc?.Invoke(context, mvc);

    internal void ConfigureSignalR(WebHostBuilderContext context, ISignalRServerBuilder signalR) =>
      _signalR?.Invoke(context, signalR);

    internal void ConfigureMvcRoutes(IRouteBuilder mvcRoutes) =>
      _mvcRoutes?.Invoke(mvcRoutes);

    internal void ConfigureSignalRRoutes(HubRouteBuilder signalRRoutes) =>
      _signalRRoutes?.Invoke(signalRRoutes);

    public ConfigureWebApp Host(Action<IWebHostBuilder> configure)
    {
      _host = configure;

      return this;
    }

    public ConfigureWebApp App(Action<IApplicationBuilder> configure)
    {
      _app = configure;

      return this;
    }

    public ConfigureWebApp AppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configure)
    {
      _appConfiguration = configure;

      return this;
    }

    public ConfigureWebApp Services(Action<WebHostBuilderContext, IServiceCollection> configure)
    {
      _services = configure;

      return this;
    }

    public ConfigureWebApp Timeline(Action<WebHostBuilderContext, ITimelineClientBuilder> configure)
    {
      _timeline = configure;

      return this;
    }

    public ConfigureWebApp EventStore(Action<WebHostBuilderContext, IEventStoreTimelineClientBuilder> configure)
    {
      _eventStore = configure;

      return this;
    }

    public ConfigureWebApp Serilog(Action<WebHostBuilderContext, LoggerConfiguration> configure)
    {
      _serilog = configure;

      return this;
    }

    public ConfigureWebApp Mvc(Action<WebHostBuilderContext, IMvcBuilder> configure)
    {
      _mvc = configure;

      return this;
    }

    public ConfigureWebApp SignalR(Action<WebHostBuilderContext, ISignalRServerBuilder> configure)
    {
      _signalR = configure;

      return this;
    }

    public ConfigureWebApp MvcRoutes(Action<IRouteBuilder> configure)
    {
      _mvcRoutes = configure;

      return this;
    }

    public ConfigureWebApp SignalRRoutes(Action<HubRouteBuilder> configure)
    {
      _signalRRoutes = configure;

      return this;
    }

    public ConfigureWebApp AppConfiguration(Action<IConfigurationBuilder> configure) =>
      AppConfiguration((host, appConfiguration) => configure(appConfiguration));

    public ConfigureWebApp Services(Action<IServiceCollection> configure) =>
      Services((host, services) => configure(services));

    public ConfigureWebApp Timeline(Action<ITimelineClientBuilder> configure) =>
      Timeline((host, timeline) => configure(timeline));

    public ConfigureWebApp EventStore(Action<IEventStoreTimelineClientBuilder> configure) =>
      EventStore((host, eventStore) => configure(eventStore));

    public ConfigureWebApp Serilog(Action<LoggerConfiguration> configure) =>
      Serilog((host, serilog) => configure(serilog));

    public ConfigureWebApp Mvc(Action<IMvcBuilder> configure) =>
      Mvc((host, mvc) => configure(mvc));

    public ConfigureWebApp SignalR(Action<ISignalRServerBuilder> configure) =>
      SignalR((host, signalR) => configure(signalR));
  }
}