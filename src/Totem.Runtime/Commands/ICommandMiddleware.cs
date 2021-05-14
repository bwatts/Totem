using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Commands
{
    public interface ICommandMiddleware
    {
        Task InvokeAsync(ICommandContext<ICommand> context, Func<Task> next, CancellationToken cancellationToken);
    }
}