using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of a flow's activity on the timeline
  /// </summary>
  internal abstract class FlowScope : Connection, IFlowScope
  {
    readonly TaskCompletionSource<Unit> _task = new TaskCompletionSource<Unit>();
    TimelinePosition _resumeCheckpoint;
    List<FlowPoint> _postResumePoints;

    protected FlowScope(FlowKey key)
    {
      Key = key;
    }

    public FlowKey Key { get; }
    public Task Task => _task.Task;
    public FlowPoint Point { get; protected set; }

    protected volatile bool Resuming;

    public void ResumeTo(TimelinePosition checkpoint)
    {
      _resumeCheckpoint = checkpoint;

      Resuming = true;
    }

    public void Push(FlowPoint point)
    {
      if(!TryEnqueue(point) && !TryFinishResume(point))
      {
        AddPostResumePoint(point);
      }
    }

    bool TryEnqueue(FlowPoint point)
    {
      if(_resumeCheckpoint.IsNone || point.Position < _resumeCheckpoint)
      {
        Enqueue(point);

        return true;
      }

      return false;
    }

    bool TryFinishResume(FlowPoint point)
    {
      if(point.Position == _resumeCheckpoint)
      {
        Enqueue(point);

        if(_postResumePoints != null)
        {
          foreach(var postResumePoint in _postResumePoints)
          {
            Enqueue(postResumePoint);
          }

          _postResumePoints = null;
        }

        _resumeCheckpoint = TimelinePosition.None;

        return true;
      }

      return false;
    }

    void AddPostResumePoint(FlowPoint point)
    {
      if(_postResumePoints == null)
      {
        _postResumePoints = new List<FlowPoint>();
      }

      _postResumePoints.Add(point);
    }

    protected abstract void Enqueue(FlowPoint point);

    //
    // Lifecycle
    //

    protected override void Close()
    {
      _task.TrySetCanceled(State.CancellationToken);
    }

    protected void CompleteTask()
    {
      _task.SetResult(Unit.Default);

      Disconnect();
    }

    protected void CompleteTask(Exception error)
    {
      _task.SetException(error);

      Disconnect();
    }
  }
}