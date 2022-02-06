namespace Totem.Topics;

public interface ITopicPipelineBuilder
{
    Id PipelineId { get; set; }

    ITopicPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : ITopicMiddleware;

    ITopicPipeline Build();
}
