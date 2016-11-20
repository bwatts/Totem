using System;
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
  /// The scope of a request's activity on the timeline
  /// </summary>
  internal sealed class RequestScope<T> : Connection, IRequestScope where T : Request
  {
    readonly TaskCompletionSource<T> _task = new TaskCompletionSource<T>();
    readonly Subject<FlowPoint> _points = new Subject<FlowPoint>();
    readonly ILifetimeScope _lifetime;
    readonly TimelineScope _timeline;
    T _request;
    FlowPoint _point;

    internal RequestScope(ILifetimeScope lifetime, TimelineScope timeline, FlowKey key)
    {
      _lifetime = lifetime;
      _timeline = timeline;
      Key = key;
    }

    public FlowKey Key { get; }

    internal Task<T> Task => _task.Task;

    public void Push(FlowPoint point)
    {
      _points.OnNext(point);
    }

    public void PushError(Exception error)
    {
      _task.SetException(error);

      Disconnect();
    }

    //
    // Lifecycle
    //

    protected override void Open()
    {
      _request = (T) Key.Type.New();

      FlowContext.Bind(_request, Key);

      Track(_points
        .ObserveOn(ThreadPoolScheduler.Instance)
        .SelectMany(OnNextPoint)
        .Subscribe());
    }

    protected override void Close()
    {
      _task.TrySetCanceled(State.CancellationToken);
    }

    async Task<Unit> OnNextPoint(FlowPoint point)
    {
      Log.Verbose("[timeline] {Position:l} => {Flow:l}", point.Position, Key);

      try
      {
        _point = point;

        await PushPoint();
      }
      catch(Exception error)
      {
        PushError(error);
      }

      return default(Unit);
    }

    //
    // Point
    //

    async Task PushPoint()
    {
      await CallWhen();

      if(_request.Context.Done)
      {
        _task.SetResult(_request);

        Disconnect();
      }
    }

    async Task CallWhen()
    {
      var flowEvent = GetFlowEvent();

      if(flowEvent != null)
      {
        using(var scope = _lifetime.BeginCallScope())
        {
          var call = new FlowCall.When(
            _point,
            flowEvent,
            scope.Resolve<IDependencySource>(),
            _timeline.ReadPrincipal(_point),
            State.CancellationToken);

          await call.Make(_request);
        }
      }
    }

    FlowEvent GetFlowEvent()
    {
      return Key.Type.Events.Get(_point.Event, strict: false);
    }
  }
}