using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
  /// Configures and runs an instance of a Totem web application
  /// </summary>
  /// <typeparam name="TArea">The type of timeline area hosted by the application</typeparam>
  internal sealed class WebAppHost<TArea> where TArea : TimelineArea, new()
  {
    readonly IWebHostBuilder _builder = WebHost.CreateDefaultBuilder();
    readonly ConfigureWebApp _configure;

    internal WebAppHost(ConfigureWebApp configure)
    {
      _configure = configure;
    }

    internal Task Run()
    {
      SetStartupAssembly();
      SetWebRoot();

      ConfigureAppConfiguration();
      ConfigureApp();
      ConfigureServices();
      ConfigureSerilog();
      ConfigureHost();
      
      return _builder.Build().RunAsync();
    }

    void SetStartupAssembly() =>
      _builder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, Assembly.GetEntryAssembly().FullName);

    void SetWebRoot() =>
      _builder.UseWebRoot("wwwroot/dist");

    void ConfigureAppConfiguration() =>
      _builder.ConfigureAppConfiguration((context, appConfiguration) =>
      {
        if(context.HostingEnvironment.IsDevelopment())
        {
          appConfiguration.AddUserSecrets(Assembly.GetEntryAssembly(), optional: true);
        }

        _configure.ConfigureAppConfiguration(context, appConfiguration);
      });

    void ConfigureApp() =>
      _builder.Configure(app =>
      {
        var environment = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Hosting.IHostingEnvironment>();

        if(environment.IsDevelopment())
        {
          app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();

        app.UseMvc(_configure.ConfigureMvcRoutes);

        app.UseSignalR(routes =>
        {
          routes.MapQueryHub();

          _configure.ConfigureSignalRRoutes(routes);
        });

        _configure.ConfigureApp(app);
      });

    void ConfigureServices() =>
      _builder.ConfigureServices((context, services) =>
      {
        services.AddTotemRuntime();

        services.AddTimelineClient<TArea>(timeline =>
        {
          var eventStore = timeline.AddEventStore().BindOptionsToConfiguration();

          _configure.ConfigureEventStore(context, eventStore);

          _configure.ConfigureTimeline(context, timeline);
        });

        var mvc = services
          .AddMvc()
          .AddCommandsAndQueries()
          .AddApplicationPart(Assembly.GetEntryAssembly());

        _configure.ConfigureMvc(context, mvc);

        var signalR = services.AddSignalR().AddQueryNotifications();

        _configure.ConfigureSignalR(context, signalR);

        _configure.ConfigureServices(context, services);
      });

    void ConfigureSerilog() =>
      _builder.UseSerilog((context, serilog) =>
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

        _configure.ConfigureSerilog(context, serilog);
      });

    void ConfigureHost() =>
      _configure.ConfigureHost(_builder);
  }
}