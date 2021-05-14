using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Queues
{
    public interface IQueueMiddleware
    {
        Task InvokeAsync(IQueueContext<IQueueCommand> context, Func<Task> next, CancellationToken cancellationToken);
    }
}