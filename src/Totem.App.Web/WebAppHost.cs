using System;
using System.Collections.Generic;
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
using Totem.Timeline.SignalR;
using Totem.Timeline.SignalR.Hosting;
using Totem.Timeline.Mvc.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.AspNetCore.Components;

namespace Totem.App.Web
{
  /// <summary>
  /// Configures and runs an instance of a Totem web application
  /// </summary>
  /// <typeparam name="TArea">The type of timeline area hosted by the application</typeparam>
  internal sealed class WebAppHost<TArea> where TArea : TimelineArea, new()
  {
    readonly IWebHostBuilder _builder = WebHost.CreateDefaultBuilder().UseEnvironment(Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? Microsoft.Extensions.Hosting.Environments.Development);
    readonly ConfigureWebApp _configure;

    internal WebAppHost(ConfigureWebApp configure)
    {
      _configure = configure;
    }

    internal Task Run()
    {
        var asm = _configure.Server;

      SetStartupAssembly(asm);
      SetWebRoot();

      ConfigureAppConfiguration(asm);
      ConfigureApp();
      ConfigureServices(asm);
      ConfigureSerilog();
      ConfigureHost();
      
      return _builder.Build().RunAsync();
    }

    void SetStartupAssembly(Assembly asm) =>
      _builder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, asm.FullName);

    void SetWebRoot() =>
      _builder.UseWebRoot("wwwroot/dist");

        void ConfigureAppConfiguration(Assembly asm) =>
      _builder.ConfigureAppConfiguration((context, appConfiguration) =>
      {
        if(context.HostingEnvironment.IsDevelopment())
        {
          appConfiguration.AddUserSecrets(asm, optional: true);
        }

        _configure.ConfigureAppConfiguration(context, appConfiguration);
      });

    void ConfigureApp() =>
      _builder.Configure(app =>
      {
          _configure.ConfigureApp(app);
      });

    void ConfigureServices(Assembly asm) =>
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
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddApplicationPart(Assembly.GetEntryAssembly())
            .AddCommandsAndQueries();

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
    void ConfigureHostConfiguration() =>
      _builder.Configure
            (hostConfiguration =>
      {
          var pairs = new Dictionary<string, string>
          {
              [HostDefaults.EnvironmentKey] = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? Microsoft.Extensions.Hosting.Environments.Development
          };

          //hostConfiguration.AddInMemoryCollection(pairs);
      });
    }
}