using Totem.Http;
using Totem.Http.Commands;

namespace Totem;

public interface IHttpCommandClient
{
    Task<IHttpCommandClientContext<IHttpCommand>> SendAsync(IHttpCommandEnvelope envelope, CancellationToken cancellationToken);
}
