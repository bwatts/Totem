using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Totem.EventBus
{
  public class EventBusHost : IHostedService
  {
    private readonly IEventBusContext _eventBusContext;

    public EventBusHost(IEventBusContext eventBusContext)
    {
      _eventBusContext = eventBusContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      await _eventBusContext.Connect();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _eventBusContext.Disconnect();
      return Task.CompletedTask;
    }
  }
}