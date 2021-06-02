using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Local
{
    public interface ILocalCommandMiddleware
    {
        Task InvokeAsync(ILocalCommandContext<ILocalCommand> context, Func<Task> next, CancellationToken cancellationToken);
    }
}