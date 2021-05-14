using Totem.Core;

namespace Totem.Events
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> Create(Id pipelineId, IEventEnvelope envelope);
    }
}