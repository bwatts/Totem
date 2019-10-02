using System;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Threading;
using Totem.Timeline.Area;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// The scope of a flow's activity on the timeline
  /// </summary>
  /// <typeparam name="T">The type of <see cref="Totem.Timeline.Flow"/> in the scope</typeparam>
  public abstract class FlowScope<T> : Connection, IFlowScope where T : Flow
  {
    readonly TaskSource<FlowResult> _lifetime = new TaskSource<FlowResult>();
    readonly FlowQueue _queue = new FlowQueue();
    bool _resumeWhenConnected;

    protected FlowScope(FlowKey key, ITimelineDb db)
    {
      Key = key;
      Db = db;
    }

    public FlowKey Key { get; }
    protected ITimelineDb Db { get; }

    public Task<FlowResult> LifetimeTask => _lifetime.Task;
    protected bool Running => !LifetimeTask.IsCompleted;
    protected bool HasPointEnqueued => _queue.HasPoint;

    protected T Flow { get; private set; }
    protected TimelinePoint Point { get; private set; }
    protected FlowObservation Observation { get; private set; }

    public void ResumeWhenConnected() =>
      _resumeWhenConnected = true;

    public void Enqueue(TimelinePoint point) =>
      _queue.Enqueue(point);

    protected override Task Open()
    {
      ObserveQueue();

      return base.Open();
    }

    protected override Task Close()
    {
      _lifetime.TrySetCanceled();

      return base.Close();
    }

    protected void CompleteTask(FlowResult result) =>
      _lifetime.SetResult(result);

    protected void CompleteTask(Exception error) =>
      _lifetime.SetException(error);

    protected async Task WriteCheckpoint()
    {
      await Db.WriteCheckpoint(Flow, Point);

      Flow.Context.SetNotNew();
    }

    void ObserveQueue() =>
      Task.Run(async () =>
      {
        if(_resumeWhenConnected)
        {
          await Resume();
        }

        while(Running)
        {
          await ObserveNextPoint();
        }
      });

    async Task Resume()
    {
      try
      {
        var info = await Db.ReadFlowToResume(Key);

        Flow = (T) info.Flow;

        _queue.ResumeWith(info.Points);
      }
      catch(Exception error)
      {
        CompleteTask(new Exception($"Error while resuming {Key}", error));
      }
    }

    async Task ObserveNextPoint()
    {
      try
      {
        Point = await _queue.Dequeue();
        Observation = Key.Type.Observations.Get(Point.Type);

        if(PointIsAfterCheckpoint)
        {
          await ObservePointIfStarted();
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

    bool PointIsAfterCheckpoint =>
      Flow == null || Point.Position > Flow.Context.CheckpointPosition;

    async Task ObservePointIfStarted()
    {
      if(Flow == null)
      {
        await TryStart();
      }

      if(Running && Flow != null)
      {
        await ObservePoint();

        if(Flow.Context.IsDone)
        {
          CompleteTask(FlowResult.Done);
        }
      }
    }

    async Task TryStart()
    {
      var info = await Db.ReadFlow(Key);

      switch(info)
      {
        case FlowInfo.NotFound notFound:
          StartIfFirst();
          break;
        case FlowInfo.Stopped stopped:
          throw new Exception($"Flow is stopped at {stopped.Position} with this error: {stopped.Error}");
        case FlowInfo.Loaded loaded:
          Flow = (T) loaded.Flow;
          break;
        default:
          throw new NotSupportedException($"Unknown resume info type {info.GetType()}");
      }
    }

    void StartIfFirst()
    {
      if(Observation.CanBeFirst)
      {
        Flow = (T) Key.Type.New();

        FlowContext.Bind(Flow, Key);
      }
      else
      {
        if(!_queue.HasPoint)
        {
          CompleteTask(FlowResult.Ignored);
        }
      }
    }

    protected abstract Task ObservePoint();

    async Task Stop(Exception error)
    {
      try
      {
        Flow.Context.SetError(Point.Position, error.ToString());

        await WriteCheckpoint();

        CompleteTask(new Exception($"Flow {Key} stopped at position {Point.Position}", error));
      }
      catch(Exception writeError)
      {
        CompleteTask(new Exception($"Failed to write stoppage of {Key} at {Point.Position} to timeline", new AggregateException(error, writeError)));
      }
    }
  }
}