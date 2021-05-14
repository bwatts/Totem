using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Routes
{
    public interface IRouteMiddleware
    {
        Task InvokeAsync(IRouteContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken);
    }
}