namespace Totem.Queries
{
    public interface IQueryPipelineBuilder
    {
        Id PipelineId { get; set; }

        IQueryPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
            where TMiddleware : IQueryMiddleware;

        IQueryPipeline Build();
    }
}