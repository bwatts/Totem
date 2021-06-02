namespace Totem.Http
{
    public interface IHttpQueryPipelineBuilder
    {
        Id PipelineId { get; set; }

        IHttpQueryPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
            where TMiddleware : IHttpQueryMiddleware;

        IHttpQueryPipeline Build();
    }
}