namespace Totem.Commands
{
    public interface ICommandPipelineBuilder
    {
        Id PipelineId { get; set; }

        ICommandPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default)
            where TMiddleware : ICommandMiddleware;

        ICommandPipeline Build();
    }
}