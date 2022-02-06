using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Commands;

public interface IClientCommandMiddleware
{
    Task InvokeAsync(IClientCommandContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken);
}
