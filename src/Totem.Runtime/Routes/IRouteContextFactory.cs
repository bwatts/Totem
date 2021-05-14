using Totem.Events;

namespace Totem.Routes
{
    public interface IRouteContextFactory
    {
        IRouteContext<IEvent> Create(Id pipelineId, IEventEnvelope envelope, IRouteAddress address);
    }
}