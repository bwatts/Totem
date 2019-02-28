using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
  /// Hosts an instance of a Totem web application
  /// </summary>
  public static class WebApp
  {
    public static Task Run<TArea>() where TArea : TimelineArea, new() =>
      CreateDefaultBuilder<TArea>().Build().RunAsync();

    public static Task Run<TArea>(Action<IServiceCollection> configureServices) where TArea : TimelineArea, new() =>
      CreateDefaultBuilder<TArea>()
      .ConfigureServices(configureServices)
      .Build()
      .RunAsync();

    public static Task Run<TArea>(Action<IWebHostBuilder, IServiceCollection> configureHostAndServices) where TArea : TimelineArea, new()
    {
      var host = CreateDefaultBuilder<TArea>();

      host.ConfigureServices(services => configureHostAndServices(host, services));

      return host.Build().RunAsync();
    }

    public static Task Run<TArea>(Action<IWebHostBuilder, IServiceCollection> configureHostAndServices, Action<IApplicationBuilder> configureApp) where TArea : TimelineArea, new()
    {
      var host = CreateDefaultBuilder<TArea>();

      host.ConfigureServices(services => configureHostAndServices(host, services));

      host.DefaultConfigure(configureApp);

      return host.Build().RunAsync();
    }

    public static IWebHostBuilder CreateDefaultBuilder<TArea>() where TArea : TimelineArea, new() =>
      WebHost
      .CreateDefaultBuilder()
      .UseEntryAssemblyHostingStartup()
      .DefaultWebApp<TArea>();

    public static IWebHostBuilder UseEntryAssemblyHostingStartup(this IWebHostBuilder host) =>
      host.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, Assembly.GetEntryAssembly().FullName);

    public static IWebHostBuilder DefaultWebApp<TArea>(this IWebHostBuilder host) where TArea : TimelineArea, new() =>
      host
      .DefaultUseSerilog()
      .DefaultUseWebRoot()
      .DefaultConfigureServices<TArea>()
      .DefaultConfigure();

    public static IWebHostBuilder DefaultUseSerilog(this IWebHostBuilder host) =>
      host.UseSerilog((context, serilog) =>
        serilog
        .WriteTo.Console()
        .MinimumLevel.Is(LogEventLevel.Verbose));

    public static IWebHostBuilder DefaultUseWebRoot(this IWebHostBuilder host) =>
      host.UseWebRoot("wwwroot/dist");

    public static IWebHostBuilder DefaultConfigureServices<TArea>(this IWebHostBuilder host) where TArea : TimelineArea, new() =>
      host.ConfigureServices(services =>
      {
        services.AddTotemRuntime();

        services.AddTimelineClient<TArea>(client => client.AddEventStore().BindOptionsToConfiguration());

        services.AddMvc().AddCommandsAndQueries().AddApplicationPart(Assembly.GetEntryAssembly());

        services.AddSignalR().AddQueryNotifications();
      });

    public static IWebHostBuilder DefaultConfigure(this IWebHostBuilder host) =>
      host.Configure(app => app.DefaultConfigure());

    public static IWebHostBuilder DefaultConfigure(this IWebHostBuilder host, Action<IApplicationBuilder> configureApp) =>
      host.Configure(app =>
      {
        app.DefaultConfigure();

        configureApp(app);
      });

    public static void DefaultConfigure(this IApplicationBuilder app)
    {
      var environment = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Hosting.IHostingEnvironment>();

      if(environment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseStaticFiles();

      app.UseMvc();

      app.UseSignalR(routes => routes.MapQueryHub());
    }
  }
}