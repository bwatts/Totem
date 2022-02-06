namespace Totem.Queues;

public interface IQueueCommandPipeline
{
    Id Id { get; }

    Task<IQueueCommandContext<IQueueCommand>> RunAsync(IQueueCommandEnvelope envelope, CancellationToken token);
}
