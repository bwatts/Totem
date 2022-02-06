using System.Threading;
using System.Threading.Tasks;

namespace Totem.Http;

public interface IHttpCommandPipeline
{
    Id Id { get; }

    Task<IHttpCommandContext<IHttpCommand>> RunAsync(IHttpCommandEnvelope envelope, CancellationToken token);
}
