namespace Totem.Local;

public interface ILocalCommandPipelineBuilder
{
    Id PipelineId { get; set; }

    ILocalCommandPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
        where TMiddleware : ILocalCommandMiddleware;

    ILocalCommandPipeline Build();
}
