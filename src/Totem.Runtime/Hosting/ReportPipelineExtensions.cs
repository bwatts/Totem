using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Reports;

namespace Totem.Hosting;

public static class ReportPipelineExtensions
{
    public static IReportPipelineBuilder Use(this IReportPipelineBuilder builder, Func<IReportContext<IEvent>, Func<Task>, CancellationToken, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use(new ReportMiddleware(middleware));
    }

    public static IReportPipelineBuilder Use(this IReportPipelineBuilder builder, Func<IReportContext<IEvent>, Func<Task>, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(middleware is null)
            throw new ArgumentNullException(nameof(middleware));

        return builder.Use((context, next, _) => middleware(context, next));
    }

    public static IReportPipelineBuilder UseWhenCall(this IReportPipelineBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use<ReportWhenMiddleware>();
    }
}
