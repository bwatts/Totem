namespace Totem.Queues
{
    public interface IQueuePipelineBuilder
    {
        Id PipelineId { get; set; }

        IQueuePipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
            where TMiddleware : IQueueMiddleware;

        IQueuePipeline Build();
    }
}