using Microsoft.Extensions.DependencyInjection;

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
          return new StreamsDbEventBus(
            context,
            type => (IIntegrationEventHandler)p.GetRequiredService(type)
          );
        });
      });
    }
  }
}
