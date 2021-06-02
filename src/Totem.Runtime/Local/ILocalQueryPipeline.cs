using System.Threading;
using System.Threading.Tasks;

namespace Totem.Local
{
    public interface ILocalQueryPipeline
    {
        Id Id { get; }

        Task<ILocalQueryContext<ILocalQuery>> RunAsync(ILocalQueryEnvelope envelope, CancellationToken token);
    }
}