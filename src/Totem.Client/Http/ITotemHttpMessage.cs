using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Http
{
    public interface ITotemHttpMessage
    {
        Task SendAsync(HttpClient client, CancellationToken cancellationToken);
    }
}