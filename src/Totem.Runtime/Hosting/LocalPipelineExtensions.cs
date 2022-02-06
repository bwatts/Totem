using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Local;

namespace Totem.Hosting;

public static class LocalPipelineExtensions
{
    public static ILocalCommandPipelineBuilder Use(this ILocalCommandPipelineBuilder builder, Func<ILocalCommandContext<ILocalCommand>, Func<Task>, CancellationToken, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use(new LocalCommandMiddleware(middleware));
    }

    public static ILocalCommandPipelineBuilder Use(this ILocalCommandPipelineBuilder builder, Func<ILocalCommandContext<ILocalCommand>, Func<Task>, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(middleware is null)
            throw new ArgumentNullException(nameof(middleware));

        return builder.Use((context, next, _) => middleware(context, next));
    }

    public static ILocalCommandPipelineBuilder UseTopic(this ILocalCommandPipelineBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use<LocalCommandTopicMiddleware>();
    }

    public static ILocalQueryPipelineBuilder Use(this ILocalQueryPipelineBuilder builder, Func<ILocalQueryContext<ILocalQuery>, Func<Task>, CancellationToken, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use(new LocalQueryMiddleware(middleware));
    }

    public static ILocalQueryPipelineBuilder Use(this ILocalQueryPipelineBuilder builder, Func<ILocalQueryContext<ILocalQuery>, Func<Task>, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(middleware is null)
            throw new ArgumentNullException(nameof(middleware));

        return builder.Use((context, next, _) => middleware(context, next));
    }

    public static ILocalQueryPipelineBuilder UseDispatcher(this ILocalQueryPipelineBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use<LocalQueryDispatcherMiddleware>();
    }
}
