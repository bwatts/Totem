using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Totem.Events;

namespace Totem
{
    public abstract class Timeline : EventSourced, ITimeline
    {
        readonly ConcurrentQueue<IEvent> _newEvents = new();

        protected Timeline(Id id) =>
            Id = id ?? throw new ArgumentNullException(nameof(id));

        public Id Id { get; }
        public bool HasNewEvents => !_newEvents.IsEmpty;
        public IEnumerable<IEvent> NewEvents => _newEvents.Select(x => x);

        protected void Then(IEvent e)
        {
            if(e == null)
                throw new ArgumentNullException(nameof(e));

            if(HasErrors)
                throw new InvalidOperationException($"Timeline {this} has one or more errors preventing {e.GetType().Name} from occurring");

            _newEvents.Enqueue(e);
        }

        protected void Then(IEnumerable<IEvent> events)
        {
            if(events == null)
                throw new ArgumentNullException(nameof(events));

            foreach(var e in events)
            {
                Then(e);
            }
        }

        protected void Then(params IEvent[] events) =>
            Then(events.AsEnumerable());
    }
}