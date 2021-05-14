using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Events
{
    public interface IEventMiddleware
    {
        Task InvokeAsync(IEventContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken);
    }
}