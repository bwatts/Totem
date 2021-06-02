using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Local;

namespace Totem.Hosting
{
    public static class LocalPipelineExtensions
    {
        public static ILocalCommandPipelineBuilder Use(this ILocalCommandPipelineBuilder builder, Func<ILocalCommandContext<ILocalCommand>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new LocalCommandMiddleware(middleware));
        }

        public static ILocalCommandPipelineBuilder Use(this ILocalCommandPipelineBuilder builder, Func<ILocalCommandContext<ILocalCommand>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static ILocalCommandPipelineBuilder UseCommandHandler(this ILocalCommandPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<LocalCommandHandlerMiddleware>();
        }

        public static ILocalQueryPipelineBuilder Use(this ILocalQueryPipelineBuilder builder, Func<ILocalQueryContext<ILocalQuery>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new LocalQueryMiddleware(middleware));
        }

        public static ILocalQueryPipelineBuilder Use(this ILocalQueryPipelineBuilder builder, Func<ILocalQueryContext<ILocalQuery>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static ILocalQueryPipelineBuilder UseQueryHandler(this ILocalQueryPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<LocalQueryHandlerMiddleware>();
        }
    }
}