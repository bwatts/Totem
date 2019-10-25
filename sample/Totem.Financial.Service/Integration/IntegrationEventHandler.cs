using Acme.ProductImport;
using System.Threading.Tasks;
using Totem.EventBus;
using Totem.Timeline.Mvc;

namespace Totem.Financial.Service
{
  public class IntegrationEventHandler:
    IIntegrationEventHandler<ImportStartedIntegrationEvent>
  {
    private readonly ICommandServer _commandServer;

    public IntegrationEventHandler(ICommandServer commandServer)
    {
      _commandServer = commandServer;
    }

    public async Task Handle(ImportStartedIntegrationEvent @event)
    {
      await _commandServer.Execute(new SetBalance(10));
    }
  }
}
