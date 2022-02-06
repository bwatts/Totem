using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Topics;

namespace Totem.Http;

public class HttpCommandTopicMiddleware : IHttpCommandMiddleware
{
    readonly ITopicPipeline _topicPipeline;

    public HttpCommandTopicMiddleware(ITopicPipeline topicPipeline) =>
        _topicPipeline = topicPipeline ?? throw new ArgumentNullException(nameof(topicPipeline));

    public async Task InvokeAsync(IHttpCommandContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

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
