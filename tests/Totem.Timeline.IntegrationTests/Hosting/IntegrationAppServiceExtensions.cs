using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Totem.Runtime.Hosting;
using Totem.Timeline.Client;
using Totem.Timeline.EventStore;
using Totem.Timeline.EventStore.Client;
using Totem.Timeline.EventStore.Hosting;
using Totem.Timeline.Hosting;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// Extends <see cref="IHostBuilder"/> to configure an integration test
  /// </summary>
  internal static class IntegrationAppServiceExtensions
  {
    static int _eventStorePortOffset = -1;

    internal static IHostBuilder ConfigureIntegrationApp(this IHostBuilder builder, IntegrationAppHost host) =>
      builder
      .ConfigureAppConfiguration(configuration => configuration.ConfigureIntegrationApp(host))
      .ConfigureServices(services => services.AddIntegrationApp(host));

    static void ConfigureIntegrationApp(this IConfigurationBuilder builder, IntegrationAppHost host) =>
      builder
      .AddJsonFile("appsettings.json")
      .AddUserSecrets(host.TestType.Assembly);

    static void AddIntegrationApp(this IServiceCollection services, IntegrationAppHost host) =>
      services
      .AddTotemRuntime()
      .AddTimeline(timeline => timeline.AddEventStore().BindOptionsToConfiguration())
      .ConfigureArea(host.GetAreaTypes())
      .ConfigureEventStorePorts()
      .AddEventStoreProcess()
      .AddSingleton<IClientDb, ClientDb>()
      .AddSingleton<IntegrationApp>()
      .AddSingleton<IHostLifetime>(p => new IntegrationAppLifetime(
        host,
        p.GetService<IntegrationApp>(),
        p.GetService<IApplicationLifetime>()))
      .Add(host.AppServices);

    static IEnumerable<Type> GetAreaTypes(this IntegrationAppHost host) =>
      host.TestType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);

    static IServiceCollection AddEventStoreProcess(this IServiceCollection services) =>
      services
      .BindOptionsToConfiguration<EventStoreProcessOptions>("eventStoreProcess")
      .AddSingleton(p =>
      {
        var processOptions = p.GetOptions<EventStoreProcessOptions>();
        var timelineOptions = p.GetOptions<EventStoreTimelineOptions>();

        Expect.That(processOptions.ExeFile).IsNot("<user secret>", "The eventStoreProcess:exeFile options is required, generally as a user secret");

        var command = new EventStoreProcessCommand(
          processOptions.ExeFile,
          timelineOptions.Server.TcpPort,
          timelineOptions.Server.HttpPort);

        return new EventStoreProcess(command, processOptions.ReadyDelay);
      });

    static IServiceCollection ConfigureEventStorePorts(this IServiceCollection services)
    {
      var offset = Interlocked.Increment(ref _eventStorePortOffset);

      return services.Configure<EventStoreTimelineOptions>(options =>
      {
        options.Server.TcpPort += offset;
        options.Server.HttpPort += offset;
      });
    }
  }
}