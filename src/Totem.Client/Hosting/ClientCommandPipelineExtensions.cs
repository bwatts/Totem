using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Commands;

namespace Totem.Hosting
{
    public static class ClientCommandPipelineExtensions
    {
        public static IClientCommandPipelineBuilder Use(this IClientCommandPipelineBuilder builder, Func<IClientCommandContext<IHttpCommand>, Func<Task>, CancellationToken, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use(new ClientCommandMiddleware(middleware));
        }

        public static IClientCommandPipelineBuilder Use(this IClientCommandPipelineBuilder builder, Func<IClientCommandContext<IHttpCommand>, Func<Task>, Task> middleware)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            return builder.Use((context, next, _) => middleware(context, next));
        }

        public static IClientCommandPipelineBuilder UseRequest(this IClientCommandPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<ClientCommandRequestMiddleware>();
        }
    }
}