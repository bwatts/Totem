using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Http;

namespace Totem.Hosting
{
    public static class HttpPipelineExtensions
    {
        public static IHttpCommandPipelineBuilder Use(this IHttpCommandPipelineBuilder builder, Func<IHttpCommandContext<IHttpCommand>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new HttpCommandMiddleware(middleware));
        }

        public static IHttpCommandPipelineBuilder Use(this IHttpCommandPipelineBuilder builder, Func<IHttpCommandContext<IHttpCommand>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IHttpCommandPipelineBuilder UseCommandHandler(this IHttpCommandPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<HttpCommandHandlerMiddleware>();
        }

        public static IHttpQueryPipelineBuilder Use(this IHttpQueryPipelineBuilder builder, Func<IHttpQueryContext<IHttpQuery>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new HttpQueryMiddleware(middleware));
        }

        public static IHttpQueryPipelineBuilder Use(this IHttpQueryPipelineBuilder builder, Func<IHttpQueryContext<IHttpQuery>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IHttpQueryPipelineBuilder UseQueryHandler(this IHttpQueryPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<HttpQueryHandlerMiddleware>();
        }
    }
}