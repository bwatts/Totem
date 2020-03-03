using Acme.ProductImport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Totem.EventBus;
using Totem.EventBus.StreamsDb;
using Totem.Runtime.Hosting;
using Totem.Timeline.Client;
using Totem.Timeline.Hosting;
using Totem.Timeline.StreamsDb.Hosting;

namespace Totem.Financial.Integration
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var builder = new HostBuilder()
      .ConfigureHostConfiguration(configHost =>
      {
      })
      .ConfigureAppConfiguration((hostingContext, config) =>
      {
      })
      .ConfigureLogging((hostingContext, logging) =>
      {
      })
      .ConfigureServices((hostContext, services) =>
      {
        services.AddTotemRuntime();

        services.AddTimelineClient<FinancialArea>(timeline =>
        {
          timeline.AddStreamsDb("", "sample21-financial");
        });

        services.AddEventBus(eventBus =>
        {
          eventBus.AddStreamsDb("", "sample21-eventbus");

          eventBus.Subscribe<ImportStartedIntegrationEvent, IntegrationEventHandler>(nameof(ImportStartedIntegrationEvent));
        });

        services.AddSingleton<IQueryNotifier, EmptyQueryNotifier>();
      });

      await builder.RunConsoleAsync();
    }
  }
}
