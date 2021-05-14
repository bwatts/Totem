using Totem.Events;

namespace Totem.InProcess
{
    public interface IInProcessEventSubscription
    {
        void Publish(IEventEnvelope envelope);
    }
}