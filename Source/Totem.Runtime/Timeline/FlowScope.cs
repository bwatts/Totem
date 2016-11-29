using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
    readonly TaskCompletionSource<Flow> _task = new TaskCompletionSource<Flow>();
    readonly Subject<Unit> _pushSignal = new Subject<Unit>();
    FlowRoute _initialRoute;
    TimelinePosition _resumeCheckpoint;
    List<FlowPoint> _postResumePoints;
    volatile bool _pushing;

    internal FlowScope(ILifetimeScope lifetime, TimelineScope timeline, FlowRoute initialRoute)
    {
      Lifetime = lifetime;
      Timeline = timeline;
      _initialRoute = initialRoute;

      Key = initialRoute.Key;
    }

    public FlowKey Key { get; }
    public Task<Flow> Task => _task.Task;
    public FlowPoint Point { get; private set; }

    protected ILifetimeScope Lifetime { get; }
    protected TimelineScope Timeline { get; }
    protected ConcurrentQueue<FlowPoint> Points { get; } = new ConcurrentQueue<FlowPoint>();
    protected Flow Flow { get; private set; }
    protected bool Resuming { get; private set; }
    protected bool NotCompleted => !Task.IsCompleted;

    //
    // Lifecycle
    //

    protected override void Open()
    {
      Track(_pushSignal
        .ObserveOn(ThreadPoolScheduler.Instance)
        .Subscribe(OnPushSignal));
    }

    protected override void Close()
    {
      _task.TrySetCanceled(State.CancellationToken);
    }

    protected void CompleteTask()
    {
      _task.SetResult(Flow);

      Disconnect();
    }

    protected void CompleteTask(Exception error)
    {
      _task.SetException(error);

      Disconnect();
    }

    //
    // Push
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

    void Enqueue(FlowPoint point)
    {
      Points.Enqueue(point);

      _pushSignal.OnNext(Unit.Default);
    }

    void OnPushSignal(Unit _)
    {
      if(!_pushing)
      {
        _pushing = true;

        Push();
      }
    }

    async Task Push()
    {
      if(FlowLoaded())
      {
        await PushPoints();
      }

      _pushing = false;
    }

    protected virtual async Task PushPoints()
    {
      FlowPoint point;

      while(NotCompleted && Points.TryDequeue(out point))
      {
        Point = point;

        await PushPoint();

        Point = null;
      }
    }

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

        using(var scope = Lifetime.BeginCallScope())
        {
          var call = new FlowCall.When(
            Point,
            flowEvent,
            scope.Resolve<IDependencySource>(),
            Timeline.ReadPrincipal(Point),
            State.CancellationToken);

          await call.Make(Flow);
        }
      }
    }

    bool TryGetFlowEvent(out FlowEvent flowEvent)
    {
      flowEvent = Key.Type.Events.Get(Point.Event, strict: false);

      return flowEvent != null;
    }

    //
    // Load
    //

    bool FlowLoaded()
    {
      if(Flow == null && _initialRoute != null)
      {
        LoadFlow();

        _initialRoute = null;
      }

      return Flow != null;
    }

    void LoadFlow()
    {
      try
      {
        TryLoadFlow();
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] [{Flow:l}] Failed to load flow", Key);

        CompleteTask(error);
      }
    }

    void TryLoadFlow()
    {
      Flow flow;

      if(!Timeline.TryReadFlow(_initialRoute, out flow))
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