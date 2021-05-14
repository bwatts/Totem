using System;
using Totem.Commands;
using Totem.Queries;

namespace Totem.Hosting
{
    public static class PipelineExtensions
    {
        public static ICommandPipelineBuilder UseHttpUser(this ICommandPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<CommandUserMiddleware>();
        }

        public static IQueryPipelineBuilder UseHttpUser(this IQueryPipelineBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.Use<QueryUserMiddleware>();
        }
    }
}