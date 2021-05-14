using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Commands
{
    public class CommandMiddleware : ICommandMiddleware
    {
        readonly Func<ICommandContext<ICommand>, Func<Task>, CancellationToken, Task> _middleware;

        public CommandMiddleware(Func<ICommandContext<ICommand>, Func<Task>, CancellationToken, Task> middleware) =>
            _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

        public Task InvokeAsync(ICommandContext<ICommand> context, Func<Task> next, CancellationToken cancellationToken) =>
            _middleware(context, next, cancellationToken);
    }

    public class CommandMiddleware<TService> : ICommandMiddleware
        where TService : ICommandMiddleware
    {
        readonly IServiceProvider _services;

        public CommandMiddleware(IServiceProvider services) =>
            _services = services ?? throw new ArgumentNullException(nameof(services));

        public Task InvokeAsync(ICommandContext<ICommand> context, Func<Task> next, CancellationToken cancellationToken) =>
            _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
    }
}