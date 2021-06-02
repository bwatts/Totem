namespace Totem.Local
{
    public interface ILocalQueryPipelineBuilder
    {
        Id PipelineId { get; set; }

        ILocalQueryPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
            where TMiddleware : ILocalQueryMiddleware;

        ILocalQueryPipeline Build();
    }
}