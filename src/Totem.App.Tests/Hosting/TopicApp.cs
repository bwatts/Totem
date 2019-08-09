using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Totem.Timeline;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// An application applying a test to a topic type
  /// </summary>
  internal sealed class TopicApp
  {
    readonly ConcurrentDictionary<Id, bool> _appendedEventIds = new ConcurrentDictionary<Id, bool>();
    readonly ExpectQueue _expectQueue = new ExpectQueue();
    readonly TopicAppTimelineDb _timelineDb;

    public TopicApp(TopicAppTimelineDb timelineDb)
    {
      _timelineDb = timelineDb;

      timelineDb.SubscribeApp(this);
    }

    internal async Task Append(Event e)
    {
      var eventId = Id.FromGuid();

      _appendedEventIds[eventId] = true;

      Event.Traits.EventId.Set(e, eventId);

      await _timelineDb.WriteFromApp(e);
    }

    internal Task<TEvent> Expect<TEvent>(ExpectTimeout timeout, bool scheduled) where TEvent : Event =>
      _expectQueue.Dequeue<TEvent>(timeout, scheduled);

    internal void OnNext(TimelinePoint point)
    {
      if(!_appendedEventIds.TryRemove(point.CommandId, out _))
      {
        _expectQueue.Enqueue(point);
      }
    }

    internal void OnError(Exception error) =>
      _expectQueue.OnError(error);
  }
}