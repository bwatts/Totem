using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Totem.Core;
using Totem.Events;

namespace Totem.InProcess
{
    public class InProcessTimelineStore : ITimelineStore
    {
        readonly ConcurrentDictionary<Id, TimelineHistory> _historiesById = new();
        readonly IInProcessEventSubscription _subscription;
        readonly IClock _clock;

        public InProcessTimelineStore(IInProcessEventSubscription subscription, IClock clock)
        {
            _subscription = subscription ?? throw new ArgumentNullException(nameof(subscription));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public async IAsyncEnumerable<IEventEnvelope> ReadTimelineAsync(Id timelineId, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if(timelineId == null)
                throw new ArgumentNullException(nameof(timelineId));

            await Task.CompletedTask; // Avoid depending on System.Linq.Async solely for .ToAsyncEnumerable()

            if(_historiesById.TryGetValue(timelineId, out var history))
            {
                foreach(var point in history.Events)
                {
                    yield return point;
                }
            }
        }

        public async IAsyncEnumerable<IEventEnvelope> SaveAsync(ITimeline timeline, Id correlationId, ClaimsPrincipal principal, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if(timeline == null)
                throw new ArgumentNullException(nameof(timeline));

            if(correlationId == null)
                throw new ArgumentNullException(nameof(correlationId));

            if(principal == null)
                throw new ArgumentNullException(nameof(principal));

            await Task.Yield();

            if(!timeline.HasNewEvents)
            {
                yield break;
            }

            var history = _historiesById.AddOrUpdate(
                timeline.Id,
                _ => AddHistory(timeline, correlationId, principal),
                (_, history) => UpdateHistory(history, timeline, correlationId, principal));

            foreach(var savedEvent in history.SavedEvents)
            {
                yield return savedEvent;
            }
        }

        TimelineHistory AddHistory(ITimeline timeline, Id correlationId, ClaimsPrincipal principal)
        {
            if(timeline.Version != null)
                throw new UnexpectedVersionException($"Expected timeline {timeline.GetType()}/{timeline.Id}@{timeline.Version}, but it does not exist");

            var history = new TimelineHistory(timeline.GetType(), timeline.Id);

            foreach(var newPoint in history.Save(timeline, correlationId, principal, _clock.UtcNow))
            {
                _subscription.Publish(newPoint);
            }

            return history;
        }

        TimelineHistory UpdateHistory(TimelineHistory history, ITimeline timeline, Id correlationId, ClaimsPrincipal principal)
        {
            if(timeline.Version == null)
                throw new UnexpectedVersionException($"Expected timeline {timeline.GetType()}/{timeline.Id} to not exist, but found version @{history.Version}");

            if(timeline.Version != history.Version)
                throw new UnexpectedVersionException($"Expected timeline {timeline.GetType()}/{timeline.Id}@{timeline.Version}, but found @{history.Version}");

            foreach(var newPoint in history.Save(timeline, correlationId, principal, _clock.UtcNow))
            {
                _subscription.Publish(newPoint);
            }

            return history;
        }

        class TimelineHistory
        {
            internal TimelineHistory(Type timelineType, Id timelineId)
            {
                TimelineType = timelineType;
                TimelineId = timelineId;
            }

            internal Type TimelineType { get; }
            internal Id TimelineId { get; }
            internal long Version => Events.Count - 1;
            internal List<IEventEnvelope> Events { get; } = new();
            internal List<IEventEnvelope> SavedEvents { get; } = new();

            internal IEnumerable<IEventEnvelope> Save(ITimeline timeline, Id correlationId, ClaimsPrincipal principal, DateTimeOffset whenOccurred)
            {
                SavedEvents.Clear();

                foreach(var newEvent in timeline.NewEvents)
                {
                    var version = Events.Count;
                    var eventId = Id.NewId();
                    var envelope = new EventEnvelope(newEvent, eventId, correlationId, principal, TimelineType, TimelineId, version, whenOccurred);

                    Events.Add(envelope);
                    SavedEvents.Add(envelope);

                    yield return envelope;
                }
            }
        }
    }
}