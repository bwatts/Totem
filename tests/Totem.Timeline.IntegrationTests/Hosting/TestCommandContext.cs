using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Totem.Timeline.Area;
using Totem.Timeline.Client;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// Appends to the timeline and expects events to occur
  /// </summary>
  internal sealed class TestCommandContext : ITimelineObserver, IDisposable
  {
    readonly ConcurrentDictionary<Id, bool> _appendedEventIds = new ConcurrentDictionary<Id, bool>();
    readonly ConcurrentQueue<TimelinePoint> _nextPoints = new ConcurrentQueue<TimelinePoint>();
    readonly AreaMap _area;
    readonly ICommandDb _commandDb;
    IDisposable _subscription;
    Exception _subscriptionDroppedError;
    PendingExpect _pendingExpect;

    public TestCommandContext(AreaMap area, ICommandDb commandDb)
    {
      _area = area;
      _commandDb = commandDb;
    }

    void IDisposable.Dispose() =>
      _subscription?.Dispose();

    Task ITimelineObserver.OnNext(TimelinePoint point)
    {
      if(!_appendedEventIds.TryRemove(point.EventId, out var _))
      {
        _nextPoints.Enqueue(point);

        _pendingExpect?.Continue();
      }

      return Task.CompletedTask;
    }

    void ITimelineObserver.OnDropped(string reason, Exception error)
    {
      _subscriptionDroppedError = new Exception($"EventStore subscription was dropped ({reason})", error);

      _pendingExpect?.Continue();
    }

    internal async Task<TimelinePosition> Append(Event e)
    {
      if(_subscription == null)
      {
        _subscription = await _commandDb.Subscribe(this);
      }

      var eventId = Id.FromGuid();

      _appendedEventIds[eventId] = true;

      Event.Traits.EventId.Set(e, eventId);

      return await _commandDb.WriteEvent(e);
    }

    internal async Task<TEvent> Expect<TEvent>(ExpectTimeout timeout, bool scheduled) where TEvent : Event
    {
      if(_subscriptionDroppedError != null)
      {
        throw _subscriptionDroppedError;
      }

      var eventType = _area.Events.Get<TEvent>();

      _pendingExpect = new PendingExpect(eventType, timeout);

      if(!_nextPoints.TryDequeue(out var nextPoint))
      {
        await _pendingExpect.WaitForNextPoint();

        _nextPoints.TryDequeue(out nextPoint);
      }

      if(nextPoint.Scheduled && !scheduled)
      {
        throw new ExpectException($"Expected an unscheduled event: {nextPoint}");
      }
      else if(!nextPoint.Scheduled && scheduled)
      {
        throw new ExpectException($"Expected a scheduled event: {nextPoint}");
      }
      else if(nextPoint.Event is TEvent e)
      {
        return e;
      }
      else
      {
        throw new ExpectException($"Expected an event of type {eventType}, received {nextPoint.Type}");
      }
    }
  }
}