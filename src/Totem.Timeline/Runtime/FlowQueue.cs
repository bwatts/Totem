using System.Collections.Generic;
using System.Threading.Tasks;
using Totem.Threading;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// A queue of the timeline points that a flow observes
  /// </summary>
  internal sealed class FlowQueue
  {
    readonly Queue<TimelinePoint> _queue = new Queue<TimelinePoint>();
    Queue<TimelinePoint> _resumeQueue = new Queue<TimelinePoint>();
    TimelinePosition _resumeCheckpoint;
    TaskSource<TimelinePoint> _pendingDequeue;

    internal bool HasPoint { get; private set; }

    internal void ResumeWith(Many<TimelinePoint> points)
    {
      HasPoint = points.Count > 0;

      foreach(var point in points)
      {
        _resumeQueue.Enqueue(point);

        _resumeCheckpoint = point.Position;
      }
    }

    internal void Enqueue(TimelinePoint point)
    {
      lock(_queue)
      {
        if(_resumeQueue == null && _pendingDequeue != null)
        {
          _pendingDequeue.SetResult(point);
          _pendingDequeue = null;
        }
        else
        {
          _queue.Enqueue(point);

          HasPoint = true;
        }
      }
    }

    internal async Task<TimelinePoint> Dequeue()
    {
      var pendingDequeue = null as TaskSource<TimelinePoint>;

      lock(_queue)
      {
        if(TryDequeueResumePoint(out var nextPoint) || TryDequeuePoint(out nextPoint))
        {
          SetHasPoint();

          return nextPoint;
        }

        pendingDequeue = new TaskSource<TimelinePoint>();

        _pendingDequeue = pendingDequeue;
      }

      return await pendingDequeue.Task;
    }

    bool TryDequeueResumePoint(out TimelinePoint nextPoint)
    {
      nextPoint = null;

      if(_resumeQueue != null)
      {
        if(_resumeQueue.Count > 0)
        {
          nextPoint = _resumeQueue.Dequeue();
        }

        if(_resumeQueue.Count == 0)
        {
          _resumeQueue = null;
        }
      }

      return nextPoint != null;
    }

    bool TryDequeuePoint(out TimelinePoint nextPoint)
    {
      nextPoint = null;

      while(_queue.Count > 0)
      {
        var point = _queue.Dequeue();

        if(IsAfterResumeCheckpoint(point))
        {
          nextPoint = point;

          break;
        }
      }

      return nextPoint != null;
    }

    void SetHasPoint() =>
      HasPoint = (_resumeQueue?.Count ?? 0) + _queue.Count > 0;

    bool IsAfterResumeCheckpoint(TimelinePoint point) =>
      _resumeCheckpoint.IsNone || point.Position > _resumeCheckpoint;
  }
}