using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of a flow's activity on the timeline
  /// </summary>
  /// <typeparam name="T">The type of flow in the scope</typeparam>
  public abstract class FlowScope<T> : Connection, IFlowScope where T : Flow
  {
    private readonly TaskCompletionSource<T> _taskCompletionSource = new TaskCompletionSource<T>();
    private readonly ILifetimeScope _lifetime;
    private readonly TimelineScope _timeline;
    private FlowQueue _queue;
    private Task _pushQueueTask;

    protected FlowScope(ILifetimeScope lifetime, TimelineScope timeline, T instance)
    {
      _lifetime = lifetime;
      _timeline = timeline;

      Instance = instance;
      Key = instance.Context.Key;
    }

    Flow IFlowScope.Instance => Instance;
    Task IFlowScope.Task => Task;

    public T Instance { get; }
    public FlowKey Key { get; }
    public FlowPoint Point { get; private set; }
    public Task<T> Task => _taskCompletionSource.Task;

    //
    // Lifecycle
    //

    protected override void Open()
    {
      _queue = new FlowQueue();
      _pushQueueTask = PushQueue();
    }

    protected override void Close()
    {
      _queue = null;
      _pushQueueTask = null;

      CancelTaskIfWaiting();
    }

    void CancelTaskIfWaiting()
    {
      _taskCompletionSource.TrySetCanceled(State.CancellationToken);
    }

    protected void CompleteTask()
    {
      _taskCompletionSource.TrySetResult(Instance);

      Disconnect();
    }

    protected void CompleteTask(Exception error)
    {
      _taskCompletionSource.TrySetException(error);

      Disconnect();
    }

    protected void CompleteTaskIfDone()
    {
      if(Instance.Context.Done)
      {
        CompleteTask();
      }
    }

    //
    // Push
    //

    public void Push(FlowPoint point)
    {
      if(PushingQueue)
      {
        _queue.Enqueue(point);
      }
    }

    bool PushingQueue => State.IsConnecting || State.IsConnected || State.IsReconnected;

    async Task PushQueue()
    {
      while(PushingQueue)
      {
        Point = await _queue.Dequeue();

        if(PushingQueue)
        {
          Log.Verbose("[timeline] {Position:l} => {Flow:l}", Point.Position, Key);

          await PushPoint();
        }
      }
    }

    protected abstract Task PushPoint();

    protected ILifetimeScope BeginCallScope()
    {
      return _lifetime.BeginCallScope();
    }

    protected FlowEvent GetFlowEvent(bool strict = true)
    {
      return Key.Type.Events.Get(Point.Event, strict);
    }

    protected ClaimsPrincipal ReadPrincipal()
    {
      return _timeline.ReadPrincipal(Point);
    }

    protected PushWhenResult PushWhen(FlowCall.When call)
    {
      return _timeline.PushWhen(Instance, call);
    }

    protected void PushStopped(Exception error)
    {
      Log.Error(error, "[timeline] Flow stopped: {Flow}", Key);

      try
      {
        Instance.Context.SetError(Point.Position);

        _timeline.PushStopped(Point, error);
      }
      finally
      {
        CompleteTask(error);
      }
    }
  }
}