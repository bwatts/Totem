using Totem.EventBus;
using Totem.ImportProcess.Service;
using Totem.Timeline;

namespace Acme.ProductImport.Topics
{
  public class IntegrationTopic : Query //Topic
  {
    private readonly IEventBus _eventBus;

    public IntegrationTopic(IEventBus eventBus)
    {
      _eventBus = eventBus;
    }

    void Given(ImportStarted e)
    {
      _eventBus.Publish(new ImportStartedIntegrationEvent());
    }
  }
}