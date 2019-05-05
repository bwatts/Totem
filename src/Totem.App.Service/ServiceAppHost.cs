using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Totem.Runtime.Hosting;
using Totem.Timeline.EventStore.Hosting;
using Totem.Timeline.Hosting;

namespace Totem.App.Service
{
  /// <summary>
  /// Configures and runs an instance of a Totem service application
  /// </summary>
  /// <typeparam name="TArea">The type of timeline area hosted by the application</typeparam>
  internal sealed class ServiceAppHost<TArea> where TArea : TimelineArea, new()
  {
    readonly HostBuilder _builder = new HostBuilder();
    readonly ConfigureServiceApp _configure;

    internal ServiceAppHost(ConfigureServiceApp configure)
    {
      _configure = configure;
    }

    internal Task Run()
    {
      ConfigureHostConfiguration();
      ConfigureAppConfiguration();
      ConfigureServices();
      ConfigureSerilog();
      ConfigureHost();

      return _builder.Build().RunAsync();
    }

    void ConfigureHostConfiguration() =>
      _builder.ConfigureHostConfiguration(hostConfiguration =>
      {
        var pairs = new Dictionary<string, string>
        {
          [HostDefaults.EnvironmentKey] = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? EnvironmentName.Development
        };

        hostConfiguration.AddInMemoryCollection(pairs);
      });

    void ConfigureAppConfiguration() =>
      _builder.ConfigureAppConfiguration((context, appConfiguration) =>
      {
        appConfiguration
        .AddEnvironmentVariables()
        .AddCommandLine(Environment.GetCommandLineArgs())
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);

        if(context.HostingEnvironment.IsDevelopment())
        {
          appConfiguration.AddUserSecrets(Assembly.GetEntryAssembly(), optional: true);
        }

        _configure.ConfigureAppConfiguration(context, appConfiguration);
      });

    void ConfigureServices() =>
      _builder.ConfigureServices((context, services) =>
      {
        services.AddTotemRuntime();

        services.AddTimeline<TArea>(timeline =>
        {
          var eventStore = timeline.AddEventStore().BindOptionsToConfiguration();

          _configure.ConfigureEventStore(context, eventStore);

          _configure.ConfigureTimeline(context, timeline);
        });

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