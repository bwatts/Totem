using Acme.ProductImport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Totem.EventBus.StreamsDb;
using Totem.Financial.Service;
using Totem.Runtime.Hosting;
using Totem.Timeline.Hosting;
using Totem.Timeline.Mvc;
using Totem.Timeline.StreamsDb.Hosting;

namespace Totem.Sample.Service
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

        services.AddTimeline<FinancialArea>(timeline =>
        {
          timeline.AddStreamsDb("", "sample07-financial");
        });

        services.AddScoped<ICommandServer, CommandServer>();
        services.AddSingleton<IntegrationEventHandler>();

        services.AddStreamsDbEventBus(eventBus =>
        {
          eventBus.Subscribe<ImportStartedIntegrationEvent, IntegrationEventHandler>(nameof(ImportStartedIntegrationEvent));
        });
        
      });

      await builder.RunConsoleAsync();
    }
  }
}
