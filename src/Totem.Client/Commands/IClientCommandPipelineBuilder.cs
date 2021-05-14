namespace Totem.Commands
{
    public interface IClientCommandPipelineBuilder
    {
        Id PipelineId { get; set; }

        IClientCommandPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
            where TMiddleware : IClientCommandMiddleware;

        IClientCommandPipeline Build();
    }
}