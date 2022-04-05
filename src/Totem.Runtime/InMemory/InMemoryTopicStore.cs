using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Totem.Core;
using Totem.Map;
using Totem.Topics;

namespace Totem.InMemory;

public class InMemoryTopicStore : ITopicStore
{
    readonly ConcurrentDictionary<ItemKey, TopicHistory> _historiesByKey = new();
    readonly IServiceProvider _services;
    readonly IInMemoryEventSubscription _subscription;
    readonly IClock _clock;

    public InMemoryTopicStore(IServiceProvider services, IInMemoryEventSubscription subscription, IClock clock)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _subscription = subscription ?? throw new ArgumentNullException(nameof(subscription));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public Task<ITopicTransaction> StartTransactionAsync(ITopicContext<ICommandMessage> context, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        var topic = (ITopic) _services.GetRequiredService(context.TopicKey.DeclaredType);

        topic.Id = context.TopicKey.Id;

        if(_historiesByKey.TryGetValue(context.TopicKey, out var history))
        {
            history.Load(topic);
        }

        return Task.FromResult<ITopicTransaction>(new TopicTransaction(this, context, topic, cancellationToken));
    }

    public Task CommitAsync(ITopicTransaction transaction, CancellationToken cancellationToken)
    {
        if(transaction is null)
            throw new ArgumentNullException(nameof(transaction));

        _historiesByKey.AddOrUpdate(
            transaction.Context.TopicKey,
            _ => AddHistory(transaction),
            (_, history) => UpdateHistory(history, transaction));

        return Task.CompletedTask;
    }

    public Task RollbackAsync(ITopicTransaction transaction, CancellationToken cancellationToken)
    {
        // Nothing to do when in memory
        return Task.CompletedTask;
    }

    TopicHistory AddHistory(ITopicTransaction transaction)
    {
        var history = new TopicHistory(transaction);

        foreach(var newPoint in history.Commit(transaction, _clock.UtcNow))
        {
            _subscription.Publish(newPoint);
        }

        return history;
    }

    TopicHistory UpdateHistory(TopicHistory history, ITopicTransaction transaction)
    {
        foreach(var newPoint in history.Commit(transaction, _clock.UtcNow))
        {
            _subscription.Publish(newPoint);
        }

        return history;
    }

    class TopicHistory
    {
        readonly List<IEvent> _events = new();
        readonly TopicType _topicType;

        internal TopicHistory(ITopicTransaction transaction) =>
            _topicType = transaction.Context.TopicType;

        internal long Version => _events.Count - 1;

        internal void Load(ITopic topic)
        {
            foreach(var e in _events)
            {
                _topicType.CallGivenIfDefined(topic, e);
            }
        }

        internal IEnumerable<IEventEnvelope> Commit(ITopicTransaction transaction, DateTimeOffset now)
        {
            foreach(var newEvent in transaction.Topic.TakeNewEvents())
            {
                var type = newEvent.GetType();
                var envelope = new EventEnvelope(
                    new ItemKey(type),
                    newEvent,
                    EventInfo.From(type),
                    transaction.Context.CommandContext.CorrelationId,
                    transaction.Context.CommandContext.Principal,
                    now);

                _events.Add(envelope.Message);

                yield return envelope;
            }
        }
    }
}
