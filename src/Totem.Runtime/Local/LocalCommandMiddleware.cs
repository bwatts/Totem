using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Local;

public class LocalCommandMiddleware : ILocalCommandMiddleware
{
    readonly Func<ILocalCommandContext<ILocalCommand>, Func<Task>, CancellationToken, Task> _middleware;

    public LocalCommandMiddleware(Func<ILocalCommandContext<ILocalCommand>, Func<Task>, CancellationToken, Task> middleware) =>
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

    public Task InvokeAsync(ILocalCommandContext<ILocalCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
        _middleware(context, next, cancellationToken);
}

public class HttpCommandMiddleware<TService> : ILocalCommandMiddleware
    where TService : ILocalCommandMiddleware
{
    readonly IServiceProvider _services;

    public HttpCommandMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task InvokeAsync(ILocalCommandContext<ILocalCommand> context, Func<Task> next, CancellationToken cancellationToken) =>
        _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
}
