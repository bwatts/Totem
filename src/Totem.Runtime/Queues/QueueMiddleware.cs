using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Queues
{
    public class QueueMiddleware : IQueueMiddleware
    {
        readonly Func<IQueueContext<IQueueCommand>, Func<Task>, CancellationToken, Task> _middleware;

        public QueueMiddleware(Func<IQueueContext<IQueueCommand>, Func<Task>, CancellationToken, Task> middleware) =>
            _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

        public Task InvokeAsync(IQueueContext<IQueueCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
            _middleware(context, next, cancellationToken);
    }

    public class QueueMiddleware<TService> : IQueueMiddleware
        where TService : IQueueMiddleware
    {
        readonly IServiceProvider _services;

        public QueueMiddleware(IServiceProvider services) =>
            _services = services ?? throw new ArgumentNullException(nameof(services));

        public Task InvokeAsync(IQueueContext<IQueueCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
            _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
    }
}