using Totem.Core;

namespace Totem.Queries
{
    public interface IQueryContextFactory
    {
        IQueryContext<IQuery> Create(Id pipelineId, IQueryEnvelope envelope);
    }
}