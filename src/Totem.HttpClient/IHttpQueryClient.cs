using Totem.Http;
using Totem.Http.Queries;

namespace Totem;

public interface IHttpQueryClient
{
    Task<IHttpQueryClientContext<IHttpQuery>> SendAsync(IHttpQueryEnvelope envelope, CancellationToken cancellationToken);
}
