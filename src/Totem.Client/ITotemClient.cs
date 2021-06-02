using System.Threading;
using System.Threading.Tasks;
using Totem.Commands;
using Totem.Http;
using Totem.Queries;

namespace Totem
{
    public interface ITotemClient
    {
        Task<IClientCommandContext<IHttpCommand>> SendAsync(IHttpCommandEnvelope envelope, CancellationToken cancellationToken);
        Task<IClientQueryContext<IHttpQuery>> SendAsync(IHttpQueryEnvelope envelope, CancellationToken cancellationToken);
    }
}