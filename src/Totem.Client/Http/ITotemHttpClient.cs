using System.Threading;
using System.Threading.Tasks;

namespace Totem.Http
{
    public interface ITotemHttpClient
    {
        Task SendAsync(ITotemHttpMessage message, CancellationToken cancellationToken);
    }
}