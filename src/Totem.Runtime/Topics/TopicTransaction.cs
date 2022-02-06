using Totem.Core;

namespace Totem.Topics;

public sealed class TopicTransaction : ITopicTransaction
{
    readonly ITopicStore _store;
    readonly CancellationToken _cancellationToken;

    public TopicTransaction(ITopicStore store, ITopicContext<ICommandMessage> context, ITopic topic, CancellationToken cancellationToken)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Topic = topic ?? throw new ArgumentNullException(nameof(topic));
        _cancellationToken = cancellationToken;
    }

    public ITopicContext<ICommandMessage> Context { get; }
    public ITopic Topic { get; }

    public async Task CommitAsync()
    {
        if(!_cancellationToken.IsCancellationRequested)
        {
            await _store.CommitAsync(this, _cancellationToken);
        }
    }

    public Task RollbackAsync() =>
        _store.RollbackAsync(this, _cancellationToken);
}
