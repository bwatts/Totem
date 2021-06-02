using System.Threading;
using System.Threading.Tasks;

namespace Totem
{
    public interface ILocalCommandHandler<in TCommand>
        where TCommand : ILocalCommand
    {
        Task HandleAsync(ILocalCommandContext<TCommand> context, CancellationToken cancellationToken);
    }
}