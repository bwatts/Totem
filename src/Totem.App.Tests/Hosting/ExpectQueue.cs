using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Totem.Threading;
using Totem.Timeline;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// Enqueues timeline points and dequeues them with expected types
  /// </summary>
  public class ExpectQueue
  {
    readonly ConcurrentQueue<TimelinePoint> _nextPoints = new ConcurrentQueue<TimelinePoint>();
    TimedWait _nextPointWait;
    Exception _error;

    public void Enqueue(TimelinePoint point)
    {
      _nextPoints.Enqueue(point);

      _nextPointWait?.OnOccurred();
    }

    public async Task<TEvent> Dequeue<TEvent>(ExpectTimeout timeout, bool scheduled) where TEvent : Event
    {
      if(_error != null)
      {
        throw new ExpectException($"Event {typeof(TEvent)} did not occur as expected", _error);
      }

      if(!_nextPoints.TryDequeue(out var nextPoint))
      {
        _nextPointWait = timeout.ToTimedWait();

        if(!_nextPoints.TryDequeue(out nextPoint))
        {
          try
          {
            await _nextPointWait.Task;
          }
          catch(Exception error)
          {
            throw new ExpectException($"Event {typeof(TEvent)} did not occur as expected", error);
          }
          finally
          {
            _nextPointWait = null;
          }

          _nextPoints.TryDequeue(out nextPoint);
        }
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
        throw new ExpectException($"Expected an event of type {typeof(TEvent)}, received {nextPoint.Type.DeclaredType}");
      }
    }

    public void OnError(Exception error)
    {
      var wait = _nextPointWait;

      if(wait != null)
      {
        wait.OnError(error);
      }
      else
      {
        _error = error;
      }
    }
  }
}