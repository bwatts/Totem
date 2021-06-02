using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Events;

namespace Totem.Hosting
{
    public static class EventPipelineExtensions
    {
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
    }
}