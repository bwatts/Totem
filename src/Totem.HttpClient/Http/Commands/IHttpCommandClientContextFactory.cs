namespace Totem.Http.Commands;

public interface IHttpCommandClientContextFactory
{
    IHttpCommandClientContext<IHttpCommand> Create(Id pipelineId, IHttpCommandEnvelope envelope);
}
