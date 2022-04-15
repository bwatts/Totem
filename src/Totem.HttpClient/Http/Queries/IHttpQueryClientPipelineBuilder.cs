namespace Totem.Http.Queries;

public interface IHttpQueryClientPipelineBuilder
{
    Id PipelineId { get; set; }

    IHttpQueryClientPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
        where TMiddleware : IHttpQueryClientMiddleware;

    IHttpQueryClientPipeline Build();
}
