using System.Threading;
using System.Threading.Tasks;
using Totem.Commands;
using Totem.Core;
using Totem.Queries;

namespace Totem
{
    public interface ITotemClient
    {
        Task<IClientCommandContext<ICommand>> SendAsync(ICommandEnvelope envelope, CancellationToken cancellationToken);
        Task<IClientQueryContext<IQuery>> SendAsync(IQueryEnvelope envelope, CancellationToken cancellationToken);
    }
}