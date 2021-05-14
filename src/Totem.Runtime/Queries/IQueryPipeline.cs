using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Queries
{
    public interface IQueryPipeline
    {
        Id Id { get; }

        Task<IQueryContext<IQuery>> RunAsync(IQueryEnvelope envelope, CancellationToken token);
    }
}