using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Core;
using Totem.Topics;

namespace Totem.Hosting;

public static class TopicPipelineExtensions
{
    public static ITopicPipelineBuilder Use(this ITopicPipelineBuilder builder, Func<ITopicContext<ICommandMessage>, Func<Task>, CancellationToken, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use(new TopicMiddleware(middleware));
    }

    public static ITopicPipelineBuilder Use(this ITopicPipelineBuilder builder, Func<ITopicContext<ICommandMessage>, Func<Task>, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(middleware is null)
            throw new ArgumentNullException(nameof(middleware));

        return builder.Use((context, next, _) => middleware(context, next));
    }

    public static ITopicPipelineBuilder UseWhen(this ITopicPipelineBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use<TopicWhenMiddleware>();
    }
}
