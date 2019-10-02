using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Totem.Timeline;
using Totem.Timeline.Area;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// An application applying a test to a topic type
  /// </summary>
  internal sealed class TopicApp
  {
    readonly ConcurrentDictionary<Id, bool> _appendedEventIds = new ConcurrentDictionary<Id, bool>();
    readonly ConcurrentDictionary<Id, TopicInstance> _instancesById = new ConcurrentDictionary<Id, TopicInstance>();
    readonly ExpectQueue _expectQueue = new ExpectQueue();
    readonly TopicAppTimelineDb _timelineDb;
    readonly TopicType _topicType;

    public TopicApp(TopicAppTimelineDb timelineDb, TopicType topicType)
    {
      _timelineDb = timelineDb;
      _topicType = topicType;

      timelineDb.SubscribeApp(this);
    }

    internal async Task Append(Event e)
    {
      var eventId = Id.FromGuid();

      _appendedEventIds[eventId] = true;

      Event.Traits.EventId.Set(e, eventId);

      var point = await _timelineDb.WriteFromApp(e);

      foreach(var route in point.Routes)
      {
        if(route.Type == _topicType)
        {
          GetInstance(route.Id).OnAppended(point);
        }
      }
    }

    internal Task<TEvent> Expect<TEvent>(ExpectTimeout timeout, bool scheduled) where TEvent : Event =>
      _expectQueue.Dequeue<TEvent>(timeout, scheduled);

    internal void OnNext(TimelinePoint point)
    {
      if(!_appendedEventIds.TryRemove(point.EventId, out _))
      {
        _expectQueue.Enqueue(point);
      }
    }

    internal void OnCheckpoint(Topic topic)
    {
      GetInstance(topic.Id).OnCheckpoint(topic);

      if(topic.Context.ErrorPosition.IsSome)
      {
        _expectQueue.OnError(new Exception(topic.Context.ErrorMessage));
      }
    }

    internal Task ExpectDone(Id instanceId, ExpectTimeout timeout) =>
      GetInstance(instanceId).ExpectDone(timeout);

    TopicInstance GetInstance(Id id)
    {
      _topicType.ExpectIdMatchesCardinality(id);

      return _instancesById.GetOrAdd(id, _ => new TopicInstance(_topicType.CreateKey(id)));
    }
  }
}