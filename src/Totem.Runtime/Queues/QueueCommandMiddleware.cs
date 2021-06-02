using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Queues
{
    public class QueueCommandMiddleware : IQueueCommandMiddleware
    {
        readonly Func<IQueueCommandContext<IQueueCommand>, Func<Task>, CancellationToken, Task> _middleware;

        public QueueCommandMiddleware(Func<IQueueCommandContext<IQueueCommand>, Func<Task>, CancellationToken, Task> middleware) =>
            _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

        public Task InvokeAsync(IQueueCommandContext<IQueueCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
            _middleware(context, next, cancellationToken);
    }

    public class QueueCommandMiddleware<TService> : IQueueCommandMiddleware
        where TService : IQueueCommandMiddleware
    {
        readonly IServiceProvider _services;

        public QueueCommandMiddleware(IServiceProvider services) =>
            _services = services ?? throw new ArgumentNullException(nameof(services));

        public Task InvokeAsync(IQueueCommandContext<IQueueCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
            _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
    }
}