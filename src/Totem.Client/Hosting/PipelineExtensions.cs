using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Commands;
using Totem.Queries;

namespace Totem.Hosting
{
    public static class PipelineExtensions
    {
        public static IClientCommandPipelineBuilder Use(this IClientCommandPipelineBuilder builder, Func<IClientCommandContext<ICommand>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new ClientCommandMiddleware(middleware));
        }

        public static IClientCommandPipelineBuilder Use(this IClientCommandPipelineBuilder builder, Func<IClientCommandContext<ICommand>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IClientCommandPipelineBuilder UseHttpRequest(this IClientCommandPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<ClientCommandHttpMiddleware>();
        }

        public static IClientQueryPipelineBuilder Use(this IClientQueryPipelineBuilder builder, Func<IClientQueryContext<IQuery>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new ClientQueryMiddleware(middleware));
        }

        public static IClientQueryPipelineBuilder Use(this IClientQueryPipelineBuilder builder, Func<IClientQueryContext<IQuery>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IClientQueryPipelineBuilder UseHttpRequest(this IClientQueryPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<ClientQueryHttpMiddleware>();
        }
    }
}