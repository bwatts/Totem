using Totem.Core;

namespace Totem.InMemory;

public interface IInMemoryEventSubscription
{
    void Publish(IEventEnvelope envelope);
}
