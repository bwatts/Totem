using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Totem.Runtime.Hosting;
using Totem.Timeline.EventStore.Hosting;
using Totem.Timeline.Hosting;
using Totem.Timeline.Mvc.Hosting;
using Totem.Timeline.SignalR.Hosting;

namespace Totem.App.Web
{
  /// <summary>
  /// Configures the hosting of a Totem web application
  /// </summary>
  public class ConfigureWebApp
  {
    class WebStep<TArg> : ConfigureStep<WebHostBuilderContext, TArg> {}

    readonly ConfigureStep<IWebHostBuilder> _host = new ConfigureStep<IWebHostBuilder>();
    readonly ConfigureStep<IApplicationBuilder> _app = new ConfigureStep<IApplicationBuilder>();
    readonly WebStep<IConfigurationBuilder> _appConfiguration = new WebStep<IConfigurationBuilder>();
    readonly WebStep<IServiceCollection> _services = new WebStep<IServiceCollection>();
    readonly WebStep<ITimelineClientBuilder> _timeline = new WebStep<ITimelineClientBuilder>();
    readonly WebStep<LoggerConfiguration> _serilog = new WebStep<LoggerConfiguration>();
    readonly WebStep<IServiceCollection> _mvc = new WebStep<IServiceCollection>();
    readonly WebStep<IServiceCollection> _signalR = new WebStep<IServiceCollection>();
    readonly ConfigureStep<IRouteBuilder> _mvcRoutes = new ConfigureStep<IRouteBuilder>();
    readonly ConfigureStep<HubRouteBuilder> _signalRRoutes = new ConfigureStep<HubRouteBuilder>();
    readonly ConfigureStep<IApplicationBuilder> _mvcApp = new ConfigureStep<IApplicationBuilder>();
    readonly ConfigureStep<IApplicationBuilder> _signalRApp = new ConfigureStep<IApplicationBuilder>();
    string _webRoot;
    bool _disableSerilog;

    public ConfigureWebApp UseWebRoot(string webRoot)
    {
      _webRoot = webRoot;

      return this;
    }

    public ConfigureWebApp DisableSerilog()
    {
      _disableSerilog = true;

      return this;
    }

    //
    // Before/After/Replace
    //

    public ConfigureWebApp BeforeHost(Action<IWebHostBuilder> configure) =>
      _host.Before(this, configure);

    public ConfigureWebApp BeforeApp(Action<IApplicationBuilder> configure) =>
      _app.Before(this, configure);

    public ConfigureWebApp BeforeAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configure) =>
      _appConfiguration.Before(this, configure);

    public ConfigureWebApp BeforeServices(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      _services.Before(this, configure);

    public ConfigureWebApp BeforeTimeline(Action<WebHostBuilderContext, ITimelineClientBuilder> configure) =>
      _timeline.Before(this, configure);

    public ConfigureWebApp BeforeSerilog(Action<WebHostBuilderContext, LoggerConfiguration> configure) =>
      _serilog.Before(this, configure);

    public ConfigureWebApp BeforeMvc(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      _mvc.Before(this, configure);

    public ConfigureWebApp BeforeSignalR(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      _signalR.Before(this, configure);

    public ConfigureWebApp BeforeMvcRoutes(Action<IRouteBuilder> configure) =>
      _mvcRoutes.Before(this, configure);

    public ConfigureWebApp BeforeSignalRRoutes(Action<HubRouteBuilder> configure) =>
      _signalRRoutes.Before(this, configure);

    public ConfigureWebApp BeforeMvcApp(Action<IApplicationBuilder> configure) =>
      _mvcApp.Before(this, configure);

    public ConfigureWebApp BeforeSignalRApp(Action<IApplicationBuilder> configure) =>
      _signalRApp.Before(this, configure);

    public ConfigureWebApp AfterHost(Action<IWebHostBuilder> configure) =>
      _host.After(this, configure);

    public ConfigureWebApp AfterApp(Action<IApplicationBuilder> configure) =>
      _app.After(this, configure);

    public ConfigureWebApp AfterAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configure) =>
      _appConfiguration.After(this, configure);

    public ConfigureWebApp AfterServices(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      _services.After(this, configure);

    public ConfigureWebApp AfterTimeline(Action<WebHostBuilderContext, ITimelineClientBuilder> configure) =>
      _timeline.After(this, configure);

    public ConfigureWebApp AfterSerilog(Action<WebHostBuilderContext, LoggerConfiguration> configure) =>
      _serilog.After(this, configure);

    public ConfigureWebApp AfterMvc(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      _mvc.After(this, configure);

    public ConfigureWebApp AfterSignalR(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      _signalR.After(this, configure);

    public ConfigureWebApp AfterMvcRoutes(Action<IRouteBuilder> configure) =>
      _mvcRoutes.After(this, configure);

    public ConfigureWebApp AfterSignalRRoutes(Action<HubRouteBuilder> configure) =>
      _signalRRoutes.After(this, configure);

    public ConfigureWebApp AfterMvcApp(Action<IApplicationBuilder> configure) =>
      _mvcApp.After(this, configure);

    public ConfigureWebApp AfterSignalRApp(Action<IApplicationBuilder> configure) =>
      _signalRApp.After(this, configure);

    public ConfigureWebApp ReplaceHost(Action<IWebHostBuilder> configure) =>
      _host.Replace(this, configure);

    public ConfigureWebApp ReplaceApp(Action<IApplicationBuilder> configure) =>
      _app.Replace(this, configure);

    public ConfigureWebApp ReplaceAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configure) =>
      _appConfiguration.Replace(this, configure);

    public ConfigureWebApp ReplaceServices(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      _services.Replace(this, configure);

    public ConfigureWebApp ReplaceTimeline(Action<WebHostBuilderContext, ITimelineClientBuilder> configure) =>
      _timeline.Replace(this, configure);

    public ConfigureWebApp ReplaceSerilog(Action<WebHostBuilderContext, LoggerConfiguration> configure) =>
      _serilog.Replace(this, configure);

    public ConfigureWebApp ReplaceMvc(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      _mvc.Replace(this, configure);

    public ConfigureWebApp ReplaceSignalR(Action<WebHostBuilderContext, IServiceCollection> configure) =>
      _signalR.Replace(this, configure);

    public ConfigureWebApp ReplaceMvcRoutes(Action<IRouteBuilder> configure) =>
      _mvcRoutes.Replace(this, configure);

    public ConfigureWebApp ReplaceSignalRRoutes(Action<HubRouteBuilder> configure) =>
      _signalRRoutes.Replace(this, configure);

    public ConfigureWebApp ReplaceMvcApp(Action<IApplicationBuilder> configure) =>
      _mvcApp.Replace(this, configure);

    public ConfigureWebApp ReplaceSignalRApp(Action<IApplicationBuilder> configure) =>
      _signalRApp.Replace(this, configure);

    //
    // Before/After/Replace (without context)
    //

    public ConfigureWebApp BeforeAppConfiguration(Action<IConfigurationBuilder> configure) =>
      _appConfiguration.Before(this, configure);

    public ConfigureWebApp BeforeServices(Action<IServiceCollection> configure) =>
      _services.Before(this, configure);

    public ConfigureWebApp BeforeTimeline(Action<ITimelineClientBuilder> configure) =>
      _timeline.Before(this, configure);

    public ConfigureWebApp BeforeSerilog(Action<LoggerConfiguration> configure) =>
      _serilog.Before(this, configure);

    public ConfigureWebApp BeforeMvc(Action<IServiceCollection> configure) =>
      _mvc.Before(this, configure);

    public ConfigureWebApp BeforeSignalR(Action<IServiceCollection> configure) =>
      _signalR.Before(this, configure);

    public ConfigureWebApp AfterAppConfiguration(Action<IConfigurationBuilder> configure) =>
      _appConfiguration.After(this, configure);

    public ConfigureWebApp AfterServices(Action<IServiceCollection> configure) =>
      _services.After(this, configure);

    public ConfigureWebApp AfterTimeline(Action<ITimelineClientBuilder> configure) =>
      _timeline.After(this, configure);

    public ConfigureWebApp AfterSerilog(Action<LoggerConfiguration> configure) =>
      _serilog.After(this, configure);

    public ConfigureWebApp AfterMvc(Action<IServiceCollection> configure) =>
      _mvc.After(this, configure);

    public ConfigureWebApp AfterSignalR(Action<IServiceCollection> configure) =>
      _signalR.After(this, configure);

    public ConfigureWebApp ReplaceAppConfiguration(Action<IConfigurationBuilder> configure) =>
      _appConfiguration.Replace(this, configure);

    public ConfigureWebApp ReplaceServices(Action<IServiceCollection> configure) =>
      _services.Replace(this, configure);

    public ConfigureWebApp ReplaceTimeline(Action<ITimelineClientBuilder> configure) =>
      _timeline.Replace(this, configure);

    public ConfigureWebApp ReplaceSerilog(Action<LoggerConfiguration> configure) =>
      _serilog.Replace(this, configure);

    public ConfigureWebApp ReplaceMvc(Action<IServiceCollection> configure) =>
      _mvc.Replace(this, configure);

    public ConfigureWebApp ReplaceSignalR(Action<IServiceCollection> configure) =>
      _signalR.Replace(this, configure);

    //
    // Apply
    //

    public void ApplyHost(IWebHostBuilder host) =>
      _host.Apply(host, () =>
      {
        host.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, Assembly.GetEntryAssembly().FullName);
        host.UseWebRoot(_webRoot ?? "wwwroot/dist");
      });

    public void ApplyApp(IWebHostBuilder host) =>
      host.Configure(app =>
        _app.Apply(app, () =>
        {
          var environment = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Hosting.IHostingEnvironment>();

          if(environment.IsDevelopment())
          {
            app.UseDeveloperExceptionPage();
          }

          app.UseStaticFiles();

          _mvcApp.Apply(app, () =>
            app.UseMvc(routes =>
              _mvcRoutes.Apply(routes)));

          _signalRApp.Apply(app, () =>
            app.UseSignalR(routes =>
              _signalRRoutes.Apply(routes, () => routes.MapQueryHub())));
        }));

    public void ApplyAppConfiguration(IWebHostBuilder host) =>
      host.ConfigureAppConfiguration((context, appConfiguration) =>
        _appConfiguration.Apply(context, appConfiguration, () =>
        {
          if(context.HostingEnvironment.IsDevelopment())
          {
            appConfiguration.AddUserSecrets(Assembly.GetEntryAssembly(), optional: true);
          }
        }));

    public void ApplyServices<TArea>(IWebHostBuilder host) where TArea : TimelineArea, new() =>
      host.ConfigureServices((context, services) =>
        _services.Apply(context, services, () =>
        {
          services.AddTotemRuntime();

          services.AddTimelineClient<TArea>(timeline =>
            _timeline.Apply(context, timeline, () =>
              timeline.AddEventStore().BindOptionsToConfiguration()));

          _mvc.Apply(context, services, () =>
            services
            .AddMvc()
            .AddTotemWebRuntime()
            .AddCommandsAndQueries()
            .AddEntryAssemblyPart());

          _signalR.Apply(context, services, () =>
            services.AddSignalR().AddQueryNotifications());
        }));

    public void ApplySerilog(IWebHostBuilder host)
    {
      if(_disableSerilog)
      {
        return;
      }

      host.UseSerilog((context, serilog) =>
        _serilog.Apply(context, serilog, () =>
        {
          if(Environment.UserInteractive)
          {
            serilog.WriteTo.Console();
          }

          if(context.HostingEnvironment.IsDevelopment())
          {
            serilog
            .MinimumLevel.Information()
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
          }
          else
          {
            serilog.MinimumLevel.Warning();
          }

          serilog.ReadFrom.Configuration(context.Configuration);
        }));
    }
  }
}