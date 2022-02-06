using Totem.Core;

namespace Totem.Topics;

public class TopicWhenMiddleware : ITopicMiddleware
{
    readonly ITopicStore _store;

    public TopicWhenMiddleware(ITopicStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public async Task InvokeAsync(ITopicContext<ICommandMessage> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        var transaction = await _store.StartTransactionAsync(context, cancellationToken);
        var command = context.CommandContext;

        if(!await command.CommandType.TryCallWhenAsync(command, transaction.Topic, cancellationToken))
        {
            context.CommandContext.Errors.Add(RuntimeErrors.CommandNotHandled);
            return;
        }

        if(!context.CommandContext.HasErrors)
        {
            await transaction.CommitAsync();
            await next();
        }
    }
}
