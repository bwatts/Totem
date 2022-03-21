using Totem.Topics;

namespace Totem.Local;

public class LocalCommandTopicMiddleware : ILocalCommandMiddleware
{
    readonly ITopicPipeline _topicPipeline;

    public LocalCommandTopicMiddleware(ITopicPipeline topicPipeline) =>
        _topicPipeline = topicPipeline ?? throw new ArgumentNullException(nameof(topicPipeline));

    public async Task InvokeAsync(ILocalCommandContext<ILocalCommand> context, Func<Task> next, CancellationToken cancellationToken)
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
