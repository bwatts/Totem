using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Queries;

namespace Totem.Hosting
{
    public static class ClientQueryPipelineExtensions
    {
        public static IClientQueryPipelineBuilder Use(this IClientQueryPipelineBuilder builder, Func<IClientQueryContext<IHttpQuery>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new ClientQueryMiddleware(middleware));
        }

        public static IClientQueryPipelineBuilder Use(this IClientQueryPipelineBuilder builder, Func<IClientQueryContext<IHttpQuery>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IClientQueryPipelineBuilder UseRequest(this IClientQueryPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<ClientQueryRequestMiddleware>();
        }
    }
}