using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Timeline.Area;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// The scope of a flow's activity on the timeline
  /// </summary>
  /// <typeparam name="T">The type of <see cref="Totem.Timeline.Flow"/> in the scope</typeparam>
  public abstract class FlowScope<T> : Connection, IFlowScope where T : Flow
  {
    readonly TaskSource<FlowResult> _taskSource = new TaskSource<FlowResult>();
    readonly Queue<TimelinePoint> _queue = new Queue<TimelinePoint>();
    Queue<TimelinePoint> _resumeQueue = new Queue<TimelinePoint>();
    TimelinePosition _resumeCheckpoint;
    TaskSource<TimelinePoint> _pendingDequeue;

    protected FlowScope(FlowKey key, ITimelineDb db)
    {
      Key = key;
      Db = db;
    }

    public FlowKey Key { get; }
    public Task<FlowResult> Task => _taskSource.Task;

    protected ITimelineDb Db { get; }
    protected bool Running => !Task.IsCompleted;

    protected T Flow { get; private set; }
    protected TimelinePoint Point { get; private set; }
    protected FlowObservation Observation { get; private set; }

    protected override Task Open()
    {
      ObserveQueue();

      return base.Open();
    }

    protected override Task Close()
    {
      _taskSource.TrySetCanceled();

      return base.Close();
    }

    void ObserveQueue() =>
      System.Threading.Tasks.Task.Run(async () =>
      {
        await Resume();

        while(Running)
        {
          await ObserveNextPoint();
        }
      });

    protected void CompleteTask(FlowResult result) =>
      _taskSource.SetResult(result);

    protected void CompleteTask(Exception error) =>
      _taskSource.SetException(error);

    //
    // Resuming
    //

    async Task Resume()
    {
      try
      {
        var info = await Db.ReadFlowResumeInfo(Key);

        switch(info)
        {
          case FlowResumeInfo.NotFound notFound:
            // .Flow remains null, which .StartFlowIfFirst checks to start the flow
            break;
          case FlowResumeInfo.Stopped stopped:
            CompleteTask(new Exception($"Flow is stopped at {stopped.Position}"));
            break;
          case FlowResumeInfo.Loaded loaded:
            Resume(loaded);
            break;
          default:
            throw new NotSupportedException($"Unknown resume info type {info.GetType()}");
        }
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] Error resuming {Key}", Key);

        CompleteTask(error);
      }
    }

    void Resume(FlowResumeInfo.Loaded info)
    {
      Flow = (T) info.Flow;

      lock(_queue)
      {
        foreach(var point in info.Points)
        {
          _resumeQueue.Enqueue(point);

          _resumeCheckpoint = point.Position;
        }
      }
    }

    //
    // Points
    //

    public void Enqueue(TimelinePoint point)
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
        }
      }
    }

    async Task ObserveNextPoint()
    {
      try
      {
        Point = await DequeueNextPoint();
        Observation = GetPointObservation();

        StartFlowIfFirst();

        if(Running && Flow != null)
        {
          await ObservePoint();

          CheckDone();
        }
      }
      catch(Exception error)
      {
        await Stop(error);
      }
      finally
      {
        Point = null;
        Observation = null;
      }
    }

    async Task<TimelinePoint> DequeueNextPoint()
    {
      var pendingDequeue = null as TaskSource<TimelinePoint>;

      lock(_queue)
      {
        if(TryDequeueResumePoint(out var nextPoint) || TryDequeuePoint(out nextPoint))
        {
          return nextPoint;
        }

        pendingDequeue = new TaskSource<TimelinePoint>();

        _pendingDequeue = pendingDequeue;
      }

      await OnPendingDequeue();

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

        if(IsAfterCheckpoint(point.Position))
        {
          nextPoint = point;

          break;
        }
      }

      return nextPoint != null;
    }

    bool IsAfterCheckpoint(TimelinePosition position) =>
      (_resumeCheckpoint.IsNone || position > _resumeCheckpoint)
      && (Flow == null || position > Flow.Context.CheckpointPosition);

    FlowObservation GetPointObservation() =>
      Key.Type.Observations.Get(Point.Type);

    void StartFlowIfFirst()
    {
      if(Flow != null)
      {
        return;
      }

      if(Observation.CanBeFirst)
      {
        Flow = (T) Key.Type.New();

        FlowContext.Bind(Flow, Key);
      }
      else
      {
        if(_queue.Count == 0)
        {
          CompleteTask(FlowResult.Ignored);
        }
      }
    }

    void CheckDone()
    {
      if(Flow.Context.Done)
      {
        CompleteTask(FlowResult.Done);
      }
    }

    protected virtual Task OnPendingDequeue() =>
      System.Threading.Tasks.Task.CompletedTask;

    protected abstract Task ObservePoint();

    //
    // Writes
    //

    protected Task WriteCheckpoint() =>
      Db.WriteCheckpoint(Flow);

    protected async Task Stop(Exception error)
    {
      Log.Error(error, "[timeline] Flow {Key} stopped", Key);

      try
      {
        SetErrorPosition();

        await WriteCheckpoint();

        CompleteTask(error);
      }
      catch(Exception writeError)
      {
        Log.Error(writeError, "[timeline] Failed to write {Key} to timeline", Key);

        CompleteTask(new AggregateException(error, writeError));
      }
    }

    void SetErrorPosition()
    {
      if(Flow != null)
      {
        Flow.Context.ErrorPosition = Point.Position;
      }
    }
  }
}