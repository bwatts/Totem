using Totem.Core;

namespace Totem.Queues
{
    public interface IQueueContextFactory
    {
        IQueueContext<IQueueCommand> Create(Id pipelineId, IQueueEnvelope envelope);
    }
}