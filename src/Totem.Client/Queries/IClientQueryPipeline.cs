using System.Threading;
using System.Threading.Tasks;
using Totem.Http;

namespace Totem.Queries;

public interface IClientQueryPipeline
{
    Id Id { get; }

    Task<IClientQueryContext<IHttpQuery>> RunAsync(IHttpQueryEnvelope envelope, CancellationToken token);
}
