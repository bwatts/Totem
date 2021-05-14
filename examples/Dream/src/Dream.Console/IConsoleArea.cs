using System.Threading;
using System.Threading.Tasks;

namespace Dream
{
    public interface IConsoleArea
    {
        Task NavigateAsync(CancellationToken cancellationToken);
    }
}