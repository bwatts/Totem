using Acme.ProductImport;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Totem.EventBus.StreamsDb;
using Totem.Runtime.Hosting;
using Totem.Timeline.Hosting;
using Totem.Timeline.StreamsDb.Hosting;
using Microsoft.Extensions.Logging;
using Totem.EventBus;

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
        logging.AddConsole();
      })
      .ConfigureServices((hostContext, services) =>
      {
        services.AddTotemRuntime();

        services.AddTimeline<ProductImportArea>(timeline =>
        {
          timeline.AddStreamsDb("", "sample21-importprocess");
        });

        services.AddEventBus(eventBus =>
        {
          eventBus.AddStreamsDb("", "sample21-eventbus");
        });

      });

      await builder.RunConsoleAsync();
    }
  }
}
