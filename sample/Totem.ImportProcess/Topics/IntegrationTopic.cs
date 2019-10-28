using Totem.EventBus;
using Totem.ImportProcess.Service;
using Totem.Timeline;

namespace Acme.ProductImport.Topics
{
  public class IntegrationTopic : Topic
  {
    void When(ImportStarted e, IEventBus eventBus)
    {
      eventBus.Publish(new ImportStartedIntegrationEvent());
    }
  }
}