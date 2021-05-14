using System.Threading;
using System.Threading.Tasks;

namespace Totem
{
    public interface IQueueHandler<in TCommand>
        where TCommand : IQueueCommand
    {
        Task HandleAsync(IQueueContext<TCommand> context, CancellationToken cancellationToken);
    }
}