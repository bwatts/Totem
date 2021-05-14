using Totem.Core;

namespace Totem.InProcess
{
    public interface IInProcessEventSubscription
    {
        void Publish(IEventEnvelope envelope);
    }
}