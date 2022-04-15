namespace Totem.Http.Commands;

public interface IHttpCommandClientPipelineBuilder
{
    Id PipelineId { get; set; }

    IHttpCommandClientPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
        where TMiddleware : IHttpCommandClientMiddleware;

    IHttpCommandClientPipeline Build();
}
