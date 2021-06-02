using System.Threading;
using System.Threading.Tasks;

namespace Totem
{
    public interface IHttpCommandHandler<in TCommand> where TCommand : IHttpCommand
    {
        Task HandleAsync(IHttpCommandContext<TCommand> context, CancellationToken cancellationToken);
    }
}