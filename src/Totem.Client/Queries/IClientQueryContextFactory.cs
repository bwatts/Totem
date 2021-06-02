using Totem.Http;

namespace Totem.Queries
{
    public interface IClientQueryContextFactory
    {
        IClientQueryContext<IHttpQuery> Create(Id pipelineId, IHttpQueryEnvelope envelope);
    }
}