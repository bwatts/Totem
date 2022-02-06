using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Queues;

namespace Totem.Hosting;

public static class QueuePipelineExtensions
{
    public static IQueueCommandPipelineBuilder Use(this IQueueCommandPipelineBuilder builder, Func<IQueueCommandContext<IQueueCommand>, Func<Task>, CancellationToken, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use(new QueueCommandMiddleware(middleware));
    }

    public static IQueueCommandPipelineBuilder Use(this IQueueCommandPipelineBuilder builder, Func<IQueueCommandContext<IQueueCommand>, Func<Task>, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(middleware is null)
            throw new ArgumentNullException(nameof(middleware));

        return builder.Use((context, next, _) => middleware(context, next));
    }

    public static IQueueCommandPipelineBuilder UseTopic(this IQueueCommandPipelineBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use<QueueCommandTopicMiddleware>();
    }
}
