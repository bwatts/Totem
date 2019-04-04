using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Totem.Runtime.Hosting;
using Totem.Timeline.Client;
using Totem.Timeline.EventStore.Client;
using Totem.Timeline.EventStore.Hosting;
using Totem.Timeline.Hosting;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// Hosts an instance of the timeline for a test
  /// </summary>
  internal static class TestHost
  {
    static int _portOffset = -1;

    internal static void Start(TestApp app) =>
      new HostBuilder()
      .ConfigureTestApp(app)
      .Build()
      .RunAsync()
      .ContinueWith(task =>
      {
        if(task.IsFaulted)
        {
          app.OnStartFailed(task.Exception);
        }
      });

    static IHostBuilder ConfigureTestApp(this IHostBuilder host, TestApp app) =>
      host
      .ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json"))
      .ConfigureServices(services =>
        services
        .AddTotemRuntime()
        .AddTimeline(timeline => timeline.AddEventStore().BindOptionsToConfiguration())
        .AddEventStoreProcess()
        .AddTestApp(app));

    static IServiceCollection AddEventStoreProcess(this IServiceCollection services) =>
      services
      .BindOptionsToConfiguration<EventStoreProcessOptions>("eventStoreProcess")
      .AddSingleton(p =>
      {
        var processOptions = p.GetOptions<EventStoreProcessOptions>();
        var timelineOptions = p.GetOptions<EventStoreTimelineOptions>();

        var command = new EventStoreProcessCommand(
          processOptions.ExeFile,
          timelineOptions.Server.TcpPort,
          timelineOptions.Server.HttpPort);

        return new EventStoreProcess(command, processOptions.ReadyDelay);
      });

    static IServiceCollection AddTestApp(this IServiceCollection services, TestApp app) =>
      services
      .ConfigureArea(app.GetAreaTypes())
      .ConfigurePorts()
      .AddSingleton<ICommandDb, CommandDb>()
      .AddSingleton<IQueryDb, QueryDb>()
      .AddSingleton<IQueryNotifier>(p => p.GetService<TestQueryNotifier>())
      .AddSingleton<TestQueryNotifier>()
      .AddSingleton<TestQueryContext>()
      .AddSingleton<TestCommandContext>()
      .AddSingleton<TestTimeline>()
      .AddSingleton<IHostLifetime>(p => new TestLifetime(
        app,
        p.GetRequiredService<TestTimeline>(),
        p.GetRequiredService<IApplicationLifetime>()));

    static IServiceCollection ConfigurePorts(this IServiceCollection services)
    {
      var offset = Interlocked.Increment(ref _portOffset);

      return services.Configure<EventStoreTimelineOptions>(options =>
      {
        options.Server.TcpPort += offset;
        options.Server.HttpPort += offset;
      });
    }
  }
}