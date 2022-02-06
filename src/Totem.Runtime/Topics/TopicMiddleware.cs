using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Totem.Core;

namespace Totem.Topics;

public class TopicMiddleware : ITopicMiddleware
{
    readonly Func<ITopicContext<ICommandMessage>, Func<Task>, CancellationToken, Task> _middleware;

    public TopicMiddleware(Func<ITopicContext<ICommandMessage>, Func<Task>, CancellationToken, Task> middleware) =>
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

    public Task InvokeAsync(ITopicContext<ICommandMessage> context, Func<Task> next, CancellationToken cancellationToken) =>
        _middleware(context, next, cancellationToken);
}

public class ReportMiddleware<TService> : ITopicMiddleware
    where TService : ITopicMiddleware
{
    readonly IServiceProvider _services;

    public ReportMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task InvokeAsync(ITopicContext<ICommandMessage> context, Func<Task> next, CancellationToken cancellationToken) =>
        _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
}
