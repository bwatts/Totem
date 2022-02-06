using Totem.Commands;
using Totem.Http;
using Totem.Queries;

namespace Totem;

public interface ITotemHttpClient
{
    Task<IClientCommandContext<IHttpCommand>> SendAsync(IHttpCommandEnvelope envelope, CancellationToken cancellationToken);
    Task<IClientQueryContext<IHttpQuery>> SendAsync(IHttpQueryEnvelope envelope, CancellationToken cancellationToken);
}
