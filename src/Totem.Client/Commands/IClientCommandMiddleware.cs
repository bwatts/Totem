using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Commands
{
    public interface IClientCommandMiddleware
    {
        Task InvokeAsync(IClientCommandContext<ICommand> context, Func<Task> next, CancellationToken cancellationToken);
    }
}