using System.Threading;
using System.Threading.Tasks;

namespace Totem.Http;

public interface IMessageClient
{
    Task SendAsync(IMessageRequest request, CancellationToken cancellationToken);
}
