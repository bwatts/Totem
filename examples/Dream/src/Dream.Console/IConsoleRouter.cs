using System.Threading;
using System.Threading.Tasks;

namespace Dream
{
    public interface IConsoleRouter
    {
        Task NavigateAsync(string route, CancellationToken cancellationToken);
    }
}