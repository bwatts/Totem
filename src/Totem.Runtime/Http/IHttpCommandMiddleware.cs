using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Http
{
    public interface IHttpCommandMiddleware
    {
        Task InvokeAsync(IHttpCommandContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken);
    }
}