using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Totem.Timeline.Hosting;

namespace Totem.App.Web
{
  /// <summary>
  /// Configures the hosting of a Totem service application
  /// </summary>
  public static class Configure
  {
    public static ConfigureWebApp DisableSerilog() =>
      new ConfigureWebApp().DisableSerilog();

    public static ConfigureWebApp BeforeHost(Action<IWebHostBuilder> configure) =>
      new ConfigureWebApp().BeforeHost(configure);

    public static ConfigureWebApp BeforeApp(Action<IApplicationBuilder> configure) =>
      new ConfigureWebApp().BeforeApp(configure);

    public static ConfigureWebApp BeforeAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configure) =>
      new ConfigureWebApp().BeforeAppConfiguration(configure);

    public static ConfigureWebApp BeforeServices(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      new ConfigureWebApp().BeforeServices(configure);

    public static ConfigureWebApp BeforeTimeline(Action<WebHostBuilderContext, ITimelineClientBuilder> configure) =>
      new ConfigureWebApp().BeforeTimeline(configure);

    public static ConfigureWebApp BeforeSerilog(Action<WebHostBuilderContext, LoggerConfiguration> configure) =>
      new ConfigureWebApp().BeforeSerilog(configure);

    public static ConfigureWebApp BeforeMvc(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      new ConfigureWebApp().BeforeMvc(configure);

    public static ConfigureWebApp BeforeSignalR(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      new ConfigureWebApp().BeforeSignalR(configure);

    public static ConfigureWebApp BeforeMvcRoutes(Action<IRouteBuilder> configure) =>
      new ConfigureWebApp().BeforeMvcRoutes(configure);

    public static ConfigureWebApp BeforeSignalRRoutes(Action<HubRouteBuilder> configure) =>
      new ConfigureWebApp().BeforeSignalRRoutes(configure);

    public static ConfigureWebApp BeforeMvcApp(Action<IApplicationBuilder> configure) =>
      new ConfigureWebApp().BeforeMvcApp(configure);

    public static ConfigureWebApp BeforeSignalRApp(Action<IApplicationBuilder> configure) =>
      new ConfigureWebApp().BeforeSignalRApp(configure);

    public static ConfigureWebApp AfterHost(Action<IWebHostBuilder> configure) =>
      new ConfigureWebApp().AfterHost(configure);

    public static ConfigureWebApp AfterApp(Action<IApplicationBuilder> configure) =>
      new ConfigureWebApp().AfterApp(configure);

    public static ConfigureWebApp AfterAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configure) =>
      new ConfigureWebApp().AfterAppConfiguration(configure);

    public static ConfigureWebApp AfterServices(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      new ConfigureWebApp().AfterServices(configure);

    public static ConfigureWebApp AfterTimeline(Action<WebHostBuilderContext, ITimelineClientBuilder> configure) =>
      new ConfigureWebApp().AfterTimeline(configure);

    public static ConfigureWebApp AfterSerilog(Action<WebHostBuilderContext, LoggerConfiguration> configure) =>
      new ConfigureWebApp().AfterSerilog(configure);

    public static ConfigureWebApp AfterMvc(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      new ConfigureWebApp().AfterMvc(configure);

    public static ConfigureWebApp AfterSignalR(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      new ConfigureWebApp().AfterSignalR(configure);

    public static ConfigureWebApp AfterMvcRoutes(Action<IRouteBuilder> configure) =>
      new ConfigureWebApp().AfterMvcRoutes(configure);

    public static ConfigureWebApp AfterSignalRRoutes(Action<HubRouteBuilder> configure) =>
      new ConfigureWebApp().AfterSignalRRoutes(configure);

    public static ConfigureWebApp AfterMvcApp(Action<IApplicationBuilder> configure) =>
      new ConfigureWebApp().AfterMvcApp(configure);

    public static ConfigureWebApp AfterSignalRApp(Action<IApplicationBuilder> configure) =>
      new ConfigureWebApp().AfterSignalRApp(configure);

    public static ConfigureWebApp ReplaceHost(Action<IWebHostBuilder> configure) =>
      new ConfigureWebApp().ReplaceHost(configure);

    public static ConfigureWebApp ReplaceApp(Action<IApplicationBuilder> configure) =>
      new ConfigureWebApp().ReplaceApp(configure);

    public static ConfigureWebApp ReplaceAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configure) =>
      new ConfigureWebApp().ReplaceAppConfiguration(configure);

    public static ConfigureWebApp ReplaceServices(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      new ConfigureWebApp().ReplaceServices(configure);

    public static ConfigureWebApp ReplaceTimeline(Action<WebHostBuilderContext, ITimelineClientBuilder> configure) =>
      new ConfigureWebApp().ReplaceTimeline(configure);

    public static ConfigureWebApp ReplaceSerilog(Action<WebHostBuilderContext, LoggerConfiguration> configure) =>
      new ConfigureWebApp().ReplaceSerilog(configure);

    public static ConfigureWebApp ReplaceMvc(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      new ConfigureWebApp().ReplaceMvc(configure);

    public static ConfigureWebApp ReplaceSignalR(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      new ConfigureWebApp().ReplaceSignalR(configure);

    public static ConfigureWebApp ReplaceMvcRoutes(Action<IRouteBuilder> configure) =>
      new ConfigureWebApp().ReplaceMvcRoutes(configure);

    public static ConfigureWebApp ReplaceSignalRRoutes(Action<HubRouteBuilder> configure) =>
      new ConfigureWebApp().ReplaceSignalRRoutes(configure);

    public static ConfigureWebApp ReplaceMvcApp(Action<IApplicationBuilder> configure) =>
      new ConfigureWebApp().ReplaceMvcApp(configure);

    public static ConfigureWebApp ReplaceSignalRApp(Action<IApplicationBuilder> configure) =>
      new ConfigureWebApp().ReplaceSignalRApp(configure);

    //
    // Without context
    //

    public static ConfigureWebApp BeforeAppConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureWebApp().BeforeAppConfiguration(configure);

    public static ConfigureWebApp BeforeServices(Action<IServiceCollection> configure) =>
      new ConfigureWebApp().BeforeServices(configure);

    public static ConfigureWebApp BeforeTimeline(Action<ITimelineClientBuilder> configure) =>
      new ConfigureWebApp().BeforeTimeline(configure);

    public static ConfigureWebApp BeforeSerilog(Action<LoggerConfiguration> configure) =>
      new ConfigureWebApp().BeforeSerilog(configure);

    public static ConfigureWebApp BeforeMvc(Action<IServiceCollection> configure) =>
      new ConfigureWebApp().BeforeMvc(configure);

    public static ConfigureWebApp BeforeSignalR(Action<IServiceCollection> configure) =>
      new ConfigureWebApp().BeforeSignalR(configure);

    public static ConfigureWebApp AfterAppConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureWebApp().AfterAppConfiguration(configure);

    public static ConfigureWebApp AfterServices(Action<IServiceCollection> configure) =>
      new ConfigureWebApp().AfterServices(configure);

    public static ConfigureWebApp AfterTimeline(Action<ITimelineClientBuilder> configure) =>
      new ConfigureWebApp().AfterTimeline(configure);

    public static ConfigureWebApp AfterSerilog(Action<LoggerConfiguration> configure) =>
      new ConfigureWebApp().AfterSerilog(configure);

    public static ConfigureWebApp AfterMvc(Action<IServiceCollection> configure) =>
      new ConfigureWebApp().AfterMvc(configure);

    public static ConfigureWebApp AfterSignalR(Action<IServiceCollection> configure) =>
      new ConfigureWebApp().AfterSignalR(configure);

    public static ConfigureWebApp ReplaceAppConfiguration(Action<IConfigurationBuilder> configure) =>
      new ConfigureWebApp().ReplaceAppConfiguration(configure);

    public static ConfigureWebApp ReplaceServices(Action<IServiceCollection> configure) =>
      new ConfigureWebApp().ReplaceServices(configure);

    public static ConfigureWebApp ReplaceTimeline(Action<ITimelineClientBuilder> configure) =>
      new ConfigureWebApp().ReplaceTimeline(configure);

    public static ConfigureWebApp ReplaceSerilog(Action<LoggerConfiguration> configure) =>
      new ConfigureWebApp().ReplaceSerilog(configure);

    public static ConfigureWebApp ReplaceMvc(Action<IServiceCollection> configure) =>
      new ConfigureWebApp().ReplaceMvc(configure);

    public static ConfigureWebApp ReplaceSignalR(Action<IServiceCollection> configure) =>
      new ConfigureWebApp().ReplaceSignalR(configure);
  }
}