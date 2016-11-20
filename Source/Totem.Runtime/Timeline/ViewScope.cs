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
  /// The scope of a view's activity on the timeline
  /// </summary>
  internal sealed class ViewScope : FlowScope
  {
    readonly ConcurrentQueue<FlowPoint> _points = new ConcurrentQueue<FlowPoint>();
    readonly Subject<Unit> _pushSignal = new Subject<Unit>();
    readonly ILifetimeScope _lifetime;
    readonly TimelineScope _timeline;
    readonly IViewExchange _exchange;
    readonly int _batchSize;
    FlowRoute _initialRoute;
    View _view;
    volatile bool _active;
    int _batchCount;

    internal ViewScope(ILifetimeScope lifetime, TimelineScope timeline, IViewExchange exchange, FlowRoute initialRoute)
      : base(initialRoute.Key)
    {
      _lifetime = lifetime;
      _timeline = timeline;
      _exchange = exchange;
      _initialRoute = initialRoute;

      _batchSize = ((ViewType) Key.Type).BatchSize;
    }

    protected override void Enqueue(FlowPoint point)
    {
      _points.Enqueue(point);

      _pushSignal.OnNext(Unit.Default);
    }

    protected override void Open()
    {
      Track(_pushSignal
        .ObserveOn(ThreadPoolScheduler.Instance)
        .SelectMany(OnPushSignal)
        .Subscribe());
    }

    async Task<Unit> OnPushSignal(Unit _)
    {
      if(!_active)
      {
        _active = true;

        if(ViewLoaded())
        {
          await PushPoints();
        }

        _active = false;
      }

      return _;
    }

    //
    // Load
    //

    bool ViewLoaded()
    {
      if(_view == null && _initialRoute != null)
      {
        LoadView();

        _initialRoute = null;
      }

      return _view != null;
    }

    void LoadView()
    {
      try
      {
        TryLoadView();
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] [{Key:l}] Failed to load view", Key);

        CompleteTask(error);
      }
    }

    void TryLoadView()
    {
      Log.Verbose("[timeline] [{Key:l}] Loading...", Key);

      Flow flow;

      if(!_timeline.TryReadFlow(_initialRoute, out flow))
      {
        Log.Verbose("[timeline] [{Key:l}] Routed view does not yet exist; ignoring", Key);

        CompleteTask();
      }
      else if(flow.Context.HasError)
      {
        Log.Verbose("[timeline] [{Key:l}] View is stopped; ignoring", Key);

        CompleteTask(new Exception($"View {Key} is stopped"));
      }
      else
      {
        _view = (View) flow;
      }
    }

    //
    // Push
    //

    bool Running => !Task.IsCompleted;

    async Task PushPoints()
    {
      FlowPoint point;

      while(Running && _points.TryDequeue(out point))
      {
        Point = point;

        AdvanceBatch();

        if(Running)
        {
          await PushPoint();
        }
      }

      CompleteBatch();
    }

    void AdvanceBatch()
    {
      if(_batchCount < _batchSize)
      {
        _batchCount++;
      }
      else
      {
        PushBatch();
      }
    }

    async Task PushPoint()
    {
      try
      {
        await CallWhen();
      }
      catch(Exception error)
      {
        if(_batchCount > 0)
        {
          PushBatch();
        }

        PushStopped(error);
      }
    }

    async Task CallWhen()
    {
      Log.Verbose("[timeline] {Position:l} => {Key:l}", Point.Position, Key);

      using(var scope = _lifetime.BeginCallScope())
      {
        var call = new FlowCall.When(
          Point,
          Key.Type.Events.Get(Point.Event),
          scope.Resolve<IDependencySource>(),
          _timeline.ReadPrincipal(Point),
          State.CancellationToken);

        await call.Make(_view);
      }
    }

    void PushBatch()
    {
      try
      {
        _timeline.PushView(_view);

        if(_batchCount > 1)
        {
          Log.Verbose("[timeline] [{Key:l}] Pushed to timeline after batch of {BatchCount}", Key, _batchCount);
        }

        _batchCount = 0;
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] [{Key:l}] Failed to push to timeline after batch of {BatchCount}", Key, _batchCount);

        CompleteTask(error);
      }
    }

    void PushStopped(Exception error)
    {
      try
      {
        _timeline.PushStopped(Point, error);
      }
      catch(Exception pushError)
      {
        Log.Error(pushError, "[timeline] [{Key:l}] Failed to push {Stopped:l} to timeline", Key, Runtime.GetEvent(typeof(FlowStopped)));

        CompleteTask(pushError);
      }
    }

    void CompleteBatch()
    {
      if(Running && _batchCount > 0)
      {
        PushBatch();
      }

      if(Running && !Resuming)
      {
        PushUpdate();
      }

      if(Running && _view.Context.Done)
      {
        CompleteTask();
      }
    }

    void PushUpdate()
    {
      try
      {
        _exchange.PushUpdate(_view);
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] [{Key:l}] Failed to push update", Key);
      }
    }
  }
}