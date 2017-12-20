using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of a flow's activity on the timeline
  /// </summary>
  internal class FlowScope : Connection, IFlowScope
  {
    readonly TaskCompletionSource<object> _task = new TaskCompletionSource<object>();
    FlowRoute _initialRoute;
    TimelinePosition _resumeCheckpoint;
    List<FlowPoint> _postResumePoints;
    Task _pushTask;
    int _pushing;

    internal FlowScope(ILifetimeScope lifetime, TimelineScope timeline, FlowRoute initialRoute)
    {
      Lifetime = lifetime;
      Timeline = timeline;
      _initialRoute = initialRoute;

      Key = initialRoute.Key;
    }

    internal FlowScope(ILifetimeScope lifetime, TimelineScope timeline, Flow flow)
    {
      Lifetime = lifetime;
      Timeline = timeline;
      Flow = flow;

      Key = flow.Context.Key;
    }

    public FlowKey Key { get; }
    public Task Task => _task.Task;
    public FlowPoint ErrorPoint { get; private set; }

    protected ILifetimeScope Lifetime { get; }
    protected TimelineScope Timeline { get; }
    protected ConcurrentQueue<FlowPoint> Points { get; } = new ConcurrentQueue<FlowPoint>();
    protected Flow Flow { get; private set; }
    protected FlowPoint Point { get; private set; }
    protected bool Resuming { get; private set; }
    protected bool NotCompleted => !Task.IsCompleted;

    //
    // Lifecycle
    //

    protected override void Close() =>
      _task.TrySetCanceled(State.CancellationToken);

    protected void CompleteTask()
    {
      _task.SetResult(null);

      Disconnect();
    }

    protected void CompleteTask(Exception error)
    {
      ErrorPoint = Point;

      _task.SetException(error);

      Disconnect();
    }

    //
    // Resume/Push
    //

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
      if(point.Position != _resumeCheckpoint)
      {
        return false;
      }

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

      Resuming = false;

      return true;
    }

    void AddPostResumePoint(FlowPoint point)
    {
      if(_postResumePoints == null)
      {
        _postResumePoints = new List<FlowPoint>();
      }

      _postResumePoints.Add(point);
    }

    //
    // Queuing

    void Enqueue(FlowPoint point)
    {
      Points.Enqueue(point);

      if(Interlocked.CompareExchange(ref _pushing, 1, 0) == 0)
      {
        _pushTask = Task.Run(PushPoints);
      }
    }

    async Task PushPoints()
    {
      while(NotCompleted && Points.TryDequeue(out var point))
      {
        await PushPoint(point);
      }

      Interlocked.Exchange(ref _pushing, 0);

      OnWaitingForPoints();
    }

    async Task PushPoint(FlowPoint point)
    {
      try
      {
        if(await FlowLoaded())
        {
          Point = point;

          TimelineMetrics.TimeToObserve.StopMeasuring($"{point.Position}/{Key}");

          await PushPoint();
        }
      }
      finally
      {
        Point = null;
      }
    }

    protected virtual void OnWaitingForPoints()
    {}

    //
    // Points
    //

    protected virtual async Task PushPoint()
    {
      try
      {
        await CallWhen();

        if(NotCompleted && Flow.Context.Done)
        {
          CompleteTask();
        }
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] [{Key:l}] Flow stopped", Key);

        Flow.Context.SetError(Point.Position);

        CompleteTask(error);
      }
    }

    protected virtual async Task CallWhen()
    {
      FlowEvent flowEvent;

      if(TryGetFlowEvent(out flowEvent))
      {
        Log.Verbose("[timeline] {Position:l} => {Flow:l}", Point.Position, Key);

        using(var lifetime = Lifetime.BeginCallScope())
        {
          await CallWhen(flowEvent, lifetime);
        }
      }
    }

    bool TryGetFlowEvent(out FlowEvent flowEvent)
    {
      flowEvent = Key.Type.Events.Get(Point.Event, strict: false);

      return flowEvent != null;
    }

    async Task CallWhen(FlowEvent flowEvent, ILifetimeScope lifetime)
    {
      var dependencies = lifetime.Resolve<IDependencySource>();

      var call = new FlowCall.When(Point, flowEvent, dependencies, State.CancellationToken);

      await call.Make(Flow);
    }

    //
    // Load
    //

    async Task<bool> FlowLoaded()
    {
      if(Flow == null && _initialRoute != null)
      {
        await LoadFlow();

        _initialRoute = null;
      }

      return Flow != null;
    }

    async Task LoadFlow()
    {
      try
      {
        await TryLoadFlow();
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] [{Flow:l}] Failed to load flow", Key);

        CompleteTask(error);
      }
    }

    async Task TryLoadFlow()
    {
      var flow = await Timeline.ReadFlow(_initialRoute, strict: false);

      if(flow == null)
      {
        CompleteTask();
      }
      else if(flow.Context.HasError)
      {
        Log.Verbose("[timeline] [{Flow:l}] Flow is stopped; ignoring", Key);

        CompleteTask(new Exception($"Flow is stopped: {Key}"));
      }
      else
      {
        Flow = flow;

        if(Flow.Context.CheckpointPosition.IsSome)
        {
          Log.Verbose("[timeline] [{Flow:l}] Loaded", Key);
        }
      }
    }
  }
}