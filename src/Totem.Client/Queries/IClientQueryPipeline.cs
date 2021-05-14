using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Queries
{
    public interface IClientQueryPipeline
    {
        Id Id { get; }

        Task<IClientQueryContext<IQuery>> RunAsync(IQueryEnvelope envelope, CancellationToken token);
    }
}