using Acme.ProductImport;
using System.Threading.Tasks;
using Totem.EventBus;
using Totem.Timeline.Client;

namespace Totem.Financial.Integration
{
  public class IntegrationEventHandler:
    IIntegrationEventHandler<ImportStartedIntegrationEvent>
  {
    private readonly IClientDb _clientDb;

    public IntegrationEventHandler(IClientDb clientDb)
    {
      _clientDb = clientDb;
    }

    public async Task Handle(ImportStartedIntegrationEvent @event)
    {
      await _clientDb.WriteEvent(new BalanceSet(10));
    }
  }
}
