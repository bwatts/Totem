using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StreamsDB.Driver;

namespace Totem.EventBus.StreamsDb
{
  public static class ServiceExtensions
  {
    public static void AddStreamsDb(this IEventBusBuilder builder, string connectionString, string stream)
    {
      builder.ConfigureServices(services =>
      {
        var context = new StreamsDbEventBusContext(connectionString, stream);

        services.AddSingleton<IEventBusContext>(context);

        services.AddSingleton<IEventBus>(p =>
        {
          var eventBus = new StreamsDbEventBus(
            context,
            type => (IIntegrationEventHandler)p.GetRequiredService(type)
          );

          eventBus.Start();

          return eventBus;
        });
      });
    }
  }

  public class StreamsDbEventBusContext : IEventBusContext
  {
    private readonly string _connectionString;
    
    public string Stream { get; }
    public StreamsDBClient Client { get; private set; }

    public StreamsDbEventBusContext(string connectionString, string stream)
    {
      _connectionString = connectionString;
      Stream = stream;
    }

    public async Task Connect()
    {
      Client = await StreamsDBClient.Connect(_connectionString);
    }

    public void Disconnect()
    {
      throw new System.NotImplementedException();
    }
  }
}
