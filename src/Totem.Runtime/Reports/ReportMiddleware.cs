using Microsoft.Extensions.DependencyInjection;

namespace Totem.Reports;

public class ReportMiddleware : IReportMiddleware
{
    readonly Func<IReportContext<IEvent>, Func<Task>, CancellationToken, Task> _middleware;

    public ReportMiddleware(Func<IReportContext<IEvent>, Func<Task>, CancellationToken, Task> middleware) =>
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));

    public Task InvokeAsync(IReportContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken) =>
        _middleware(context, next, cancellationToken);
}

public class ReportMiddleware<TService> : IReportMiddleware
    where TService : IReportMiddleware
{
    readonly IServiceProvider _services;

    public ReportMiddleware(IServiceProvider services) =>
        _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task InvokeAsync(IReportContext<IEvent> context, Func<Task> next, CancellationToken cancellationToken) =>
        _services.GetRequiredService<TService>().InvokeAsync(context, next, cancellationToken);
}
