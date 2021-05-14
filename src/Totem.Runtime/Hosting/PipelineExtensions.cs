using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Commands;
using Totem.Events;
using Totem.Queries;
using Totem.Queues;
using Totem.Reports;
using Totem.Routes;
using Totem.Workflows;

namespace Totem.Hosting
{
    public static class PipelineExtensions
    {
        public static ICommandPipelineBuilder Use(this ICommandPipelineBuilder builder, Func<ICommandContext<ICommand>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new CommandMiddleware(middleware));
        }

        public static ICommandPipelineBuilder Use(this ICommandPipelineBuilder builder, Func<ICommandContext<ICommand>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static ICommandPipelineBuilder UseCommandHandler(this ICommandPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<CommandHandlerMiddleware>();
        }

        public static ICommandPipelineBuilder UseCorrelation(this ICommandPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<CommandCorrelationMiddleware>();
        }

        //
        // Queries
        //

        public static IQueryPipelineBuilder Use(this IQueryPipelineBuilder builder, Func<IQueryContext<IQuery>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new QueryMiddleware(middleware));
        }

        public static IQueryPipelineBuilder Use(this IQueryPipelineBuilder builder, Func<IQueryContext<IQuery>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IQueryPipelineBuilder UseQueryHandler(this IQueryPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<QueryHandlerMiddleware>();
        }

        public static IQueryPipelineBuilder UseCorrelation(this IQueryPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<QueryCorrelationMiddleware>();
        }

        //
        // Queues
        //

        public static IQueuePipelineBuilder Use(this IQueuePipelineBuilder builder, Func<IQueueContext<IQueueCommand>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new QueueMiddleware(middleware));
        }

        public static IQueuePipelineBuilder Use(this IQueuePipelineBuilder builder, Func<IQueueContext<IQueueCommand>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IQueuePipelineBuilder UseQueueHandler(this IQueuePipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<QueueHandlerMiddleware>();
        }

        public static IQueuePipelineBuilder UseCorrelation(this IQueuePipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<QueueCorrelationMiddleware>();
        }

        //
        // Events
        //

        public static IEventPipelineBuilder Use(this IEventPipelineBuilder builder, Func<IEventContext<IEvent>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new EventMiddleware(middleware));
        }

        public static IEventPipelineBuilder Use(this IEventPipelineBuilder builder, Func<IEventContext<IEvent>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IEventPipelineBuilder UseEventHandlers(this IEventPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<EventHandlerMiddleware>();
        }

        public static IEventPipelineBuilder UseWorkflows(this IEventPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<WorkflowsMiddleware>();
        }

        public static IEventPipelineBuilder UseReports(this IEventPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<ReportsMiddleware>();
        }

        //
        // Workflows
        //

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

        //
        // Reports
        //

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