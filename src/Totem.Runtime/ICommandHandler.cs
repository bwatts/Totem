using System.Threading;
using System.Threading.Tasks;

namespace Totem
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(ICommandContext<TCommand> context, CancellationToken cancellationToken);
    }
}