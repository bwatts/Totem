using System.Collections.Concurrent;
using Totem.Core;
using Totem.Topics;

namespace Totem;

public abstract class Topic : Timeline, ITopic
{
    readonly ConcurrentQueue<IEvent> _newEvents = new();

    public bool HasNewEvents => !_newEvents.IsEmpty;
    public IEnumerable<IEvent> NewEvents => _newEvents.Select(x => x);

    protected void Then(IEvent e)
    {
        if(e is null)
            throw new ArgumentNullException(nameof(e));

        if(HasErrors)
            throw new InvalidOperationException($"Topic {this} has one or more errors preventing {e.GetType().Name} from occurring");

        _newEvents.Enqueue(e);
    }

    protected void Then(IEnumerable<IEvent> events)
    {
        foreach(var e in events ?? throw new ArgumentNullException(nameof(events)))
        {
            Then(e);
        }
    }

    protected void Then(params IEvent[] events) =>
        Then(events.AsEnumerable());
}
