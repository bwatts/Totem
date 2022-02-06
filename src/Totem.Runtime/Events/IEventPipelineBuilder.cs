namespace Totem.Events;

public interface IEventPipelineBuilder
{
    Id PipelineId { get; set; }

    IEventPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IEventMiddleware;

    IEventPipeline Build();
}
