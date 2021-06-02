using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Reports;
using Totem.Routes;
using Totem.Workflows;

namespace Totem.Hosting
{
    public static class RoutePipelineExtensions
    {
        public static IWorkflowPipelineBuilder Use(this IWorkflowPipelineBuilder builder, Func<IRouteContext<IEvent>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new RouteMiddleware(middleware));
        }

        public static IWorkflowPipelineBuilder Use(this IWorkflowPipelineBuilder builder, Func<IRouteContext<IEvent>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IWorkflowPipelineBuilder UseRouteHandler(this IWorkflowPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<WorkflowMiddleware>();
        }

        public static IReportPipelineBuilder Use(this IReportPipelineBuilder builder, Func<IRouteContext<IEvent>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new RouteMiddleware(middleware));
        }

        public static IReportPipelineBuilder Use(this IReportPipelineBuilder builder, Func<IRouteContext<IEvent>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IReportPipelineBuilder UseRouteHandler(this IReportPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<ReportMiddleware>();
        }
    }
}