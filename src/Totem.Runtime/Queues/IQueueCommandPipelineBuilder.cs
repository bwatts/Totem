namespace Totem.Queues;

public interface IQueueCommandPipelineBuilder
{
    Id PipelineId { get; set; }

    IQueueCommandPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
        where TMiddleware : IQueueCommandMiddleware;

    IQueueCommandPipeline Build();
}
