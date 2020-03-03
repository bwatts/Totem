using Acme.ProductImport;
using Acme.ProductImport.Topics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Totem.Runtime.Hosting;
using Totem.Timeline.Client;
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
          timeline.AddStreamsDb("", "sample21-financial");
        });

        services.AddSingleton<FinancialProcess>();

        services.AddScoped<IQueryServer, QueryServer>();
        services.AddScoped<ICommandServer, CommandServer>();
        services.AddSingleton<IQueryNotifier, EmptyQueryNotifier>();

      });

      await builder.RunConsoleAsync();
    }
  }
}
