using Totem.Core;

namespace Totem.Queries
{
    public interface IClientQueryContextFactory
    {
        IClientQueryContext<IQuery> Create(Id pipelineId, IQueryEnvelope envelope);
    }
}