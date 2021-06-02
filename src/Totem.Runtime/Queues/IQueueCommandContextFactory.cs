namespace Totem.Queues
{
    public interface IQueueCommandContextFactory
    {
        IQueueCommandContext<IQueueCommand> Create(Id pipelineId, IQueueCommandEnvelope envelope);
    }
}