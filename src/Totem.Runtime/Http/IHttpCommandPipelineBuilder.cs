namespace Totem.Http;

public interface IHttpCommandPipelineBuilder
{
    Id PipelineId { get; set; }

    IHttpCommandPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
        where TMiddleware : IHttpCommandMiddleware;

    IHttpCommandPipeline Build();
}
