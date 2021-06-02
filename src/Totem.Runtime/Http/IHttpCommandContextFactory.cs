namespace Totem.Http
{
    public interface IHttpCommandContextFactory
    {
        IHttpCommandContext<IHttpCommand> Create(Id pipelineId, IHttpCommandEnvelope envelope);
    }
}