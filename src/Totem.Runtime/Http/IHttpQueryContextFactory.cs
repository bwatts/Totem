namespace Totem.Http;

public interface IHttpQueryContextFactory
{
    IHttpQueryContext<IHttpQuery> Create(Id pipelineId, IHttpQueryEnvelope envelope);
}
