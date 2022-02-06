using System;
using System.Threading;
using System.Threading.Tasks;
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






        // This is coming through as IQueueCommandContext but the command type only has IHttpCommandContext




        if(!context.CommandType.TryCallRoute(context, out var topicKey))
        {
            context.AddError(RuntimeErrors.CommandNotHandled);
            return;
        }

        await _topicPipeline.RunAsync(context, topicKey, cancellationToken);

        if(!context.HasErrors)
        {
            await next();
        }
    }
}
