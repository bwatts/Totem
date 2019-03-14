using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Totem.Runtime.Hosting;
using Totem.Timeline.EventStore.Hosting;
using Totem.Timeline.Hosting;

namespace Totem.App.Service
{
  /// <summary>
  /// Hosts an instance of a Totem service application
  /// </summary>
  public static class ServiceApp
  {
    public static Task Run<TArea>(Action<IServiceCollection> configureServices) where TArea : TimelineArea, new() =>
      new HostBuilder()
      .DefaultServiceApp<TArea>()
      .ConfigureServices(configureServices)
      .RunConsoleAsync();

    public static Task Run<TArea>(Action<IHostBuilder, IServiceCollection> configureHostAndServices) where TArea : TimelineArea, new()
    {
      var host = new HostBuilder().DefaultServiceApp<TArea>();

      host.ConfigureServices(services => configureHostAndServices(host, services));

      return host.RunConsoleAsync();
    }

    public static IHostBuilder DefaultServiceApp<TArea>(this IHostBuilder host) where TArea : TimelineArea, new() =>
      host
      .DefaultConfigureAppConfiguration()
      .DefaultUseSerilog()
      .DefaultConfigureServices<TArea>();

    public static IHostBuilder DefaultConfigureAppConfiguration(this IHostBuilder host) =>
      host.ConfigureAppConfiguration(config =>
      {
        config.AddJsonFile("appsettings.json", optional: true);
        config.AddEnvironmentVariables();
        config.AddCommandLine(Environment.GetCommandLineArgs());

        var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

        var isDevelopment = string.IsNullOrWhiteSpace(environment) || environment.Equals("Development", StringComparison.OrdinalIgnoreCase);

        if(isDevelopment)
        {
          config.AddUserSecrets(Assembly.GetEntryAssembly());
        }
      });

    public static IHostBuilder DefaultUseSerilog(this IHostBuilder host) =>
      host.UseSerilog((context, serilog) =>
        serilog
        .WriteTo.Console()
        .MinimumLevel.Is(LogEventLevel.Verbose));

    public static IHostBuilder DefaultConfigureServices<TArea>(this IHostBuilder host) where TArea : TimelineArea, new() =>
      host.ConfigureServices(services =>
        services
        .AddOptions()
        .AddTotemRuntime()
        .AddTimeline<TArea>(timeline => timeline.AddEventStore().BindOptionsToConfiguration()));
  }
}