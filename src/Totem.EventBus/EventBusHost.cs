using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Totem.EventBus
{
  public class EventBusHost : IHostedService
  {
    private readonly IEventBusContext _eventBusContext;
    private readonly IEventBus _eventBus;
    private readonly List<SubscriptionInfo> _subscriptions;

    public EventBusHost(IEventBusContext eventBusContext, IEventBus eventBus, List<SubscriptionInfo> subscriptions)
    {
      _eventBusContext = eventBusContext;
      _eventBus = eventBus;
      _subscriptions = subscriptions;
    }
      
    public async Task StartAsync(CancellationToken cancellationToken)
    {
      await _eventBusContext.Connect();
      await _eventBus.Start(_subscriptions);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _eventBusContext.Disconnect();
      return Task.CompletedTask;
    }
  }
}