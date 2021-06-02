using System.Threading;
using System.Threading.Tasks;

namespace Totem
{
    public interface IQueueCommandHandler<in TCommand>
        where TCommand : IQueueCommand
    {
        Task HandleAsync(IQueueCommandContext<TCommand> context, CancellationToken cancellationToken);
    }
}