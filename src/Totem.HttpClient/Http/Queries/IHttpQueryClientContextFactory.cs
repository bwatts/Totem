namespace Totem.Http.Queries;

public interface IHttpQueryClientContextFactory
{
    IHttpQueryClientContext<IHttpQuery> Create(Id pipelineId, IHttpQueryEnvelope envelope);
}
