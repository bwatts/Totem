using Totem.Topics;

namespace Totem.Queues;

public class QueueCommandTopicMiddleware : IQueueCommandMiddleware
{
    readonly ITopicPipeline _topicPipeline;

    public QueueCommandTopicMiddleware(ITopicPipeline topicPipeline) =>
        _topicPipeline = topicPipeline ?? throw new ArgumentNullException(nameof(topicPipeline));

    public async Task InvokeAsync(IQueueCommandContext<IQueueCommand> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        var topicKey = context.CommandType.CallRoute(context);

        await _topicPipeline.RunAsync(context, topicKey, cancellationToken);

        if(!context.HasErrors)
        {
            await next();
        }
    }
}
