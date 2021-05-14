namespace Totem.Queries
{
    public interface IClientQueryPipelineBuilder
    {
        Id PipelineId { get; set; }

        IClientQueryPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
            where TMiddleware : IClientQueryMiddleware;

        IClientQueryPipeline Build();
    }
}