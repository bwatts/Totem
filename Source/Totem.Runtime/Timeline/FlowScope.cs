using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of a flow's activity on the timeline
  /// </summary>
  public sealed class FlowScope : Connection, IFlowScope
  {
    private readonly TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();
    private readonly ILifetimeScope _lifetime;
    private readonly TimelineScope _timeline;
    private readonly IViewExchange _viewExchange;
    private FlowQueue _queue;
    private Task _pushQueueTask;

    public FlowScope(ILifetimeScope lifetime, TimelineScope timeline, IViewExchange viewExchange, Flow instance)
    {
      _lifetime = lifetime;
      _timeline = timeline;
      _viewExchange = viewExchange;

      Instance = instance;
      Key = instance.Context.Key;
    }

    public Flow Instance { get; }
    public FlowKey Key { get; }
    public FlowPoint Point { get; private set; }
    public Task Task => _taskCompletionSource.Task;

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

    private void CancelTaskIfWaiting()
    {
      _taskCompletionSource.TrySetCanceled(State.CancellationToken);
    }

    private void CompleteTask()
    {
      _taskCompletionSource.TrySetResult(null);

      Disconnect();
    }

    private void CompleteTask(Exception error)
    {
      _taskCompletionSource.TrySetException(error);

      Disconnect();
    }

    private void CompleteTaskIfDone()
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

    private bool PushingQueue => State.IsConnecting || State.IsConnected || State.IsReconnected;

    private async Task PushQueue()
    {
      while(PushingQueue)
      {
        Point = await _queue.Dequeue();

        if(PushingQueue)
        {
          await PushNext();
        }
      }
    }

    private async Task PushNext()
    {
      Log.Verbose("[timeline] {Position:l} => {Flow:l}", Point.Position, Key);

      try
      {
        var flowEvent = Key.Type.Events.Get(Point.Event);

        CallGivenIfTopic(flowEvent);

        await CallWhen(flowEvent);

        PushUpdateIfView();

        CompleteTaskIfDone();
      }
      catch(Exception error)
      {
        if(Key.Type.IsRequest)
        {
          CompleteTask(error);
        }
        else
        {
          PushStopped(error);
        }
      }
    }

    private void CallGivenIfTopic(FlowEvent flowEvent)
    {
      var topic = Instance as Topic;

      if(topic != null && Point.Route.Given && !Point.Route.Then)
      {
        var call = new FlowCall.Given(Point, (TopicEvent) flowEvent);

        call.Make(topic);
      }
    }

    private async Task CallWhen(FlowEvent flowEvent)
    {
      using(var scope = _lifetime.BeginCallScope())
      {
        await (Key.Type.IsTopic
          ? CallTopicWhen(flowEvent, scope)
          : CallWhen(flowEvent, scope));
      }
    }

    private async Task CallTopicWhen(FlowEvent flowEvent, ILifetimeScope scope)
    {
      var call = new FlowCall.TopicWhen(
        Point,
        (TopicEvent) flowEvent,
        scope.Resolve<IDependencySource>(),
        _timeline.ReadPrincipal(Point),
        State.CancellationToken);

      await call.Make(Instance);

      PushWhen(call);
    }

    private async Task CallWhen(FlowEvent flowEvent, ILifetimeScope scope)
    {
      var call = new FlowCall.When(
        Point,
        flowEvent,
        scope.Resolve<IDependencySource>(),
        _timeline.ReadPrincipal(Point),
        State.CancellationToken);

      await call.Make(Instance);

      if(!Key.Type.IsRequest)
      {
        PushWhen(call);
      }
    }

    private void PushWhen(FlowCall.When call)
    {
      var result = _timeline.PushWhen(Instance, call);

      if(result.FlowStopped)
      {
        CompleteTask();
      }
    }

    private void PushUpdateIfView()
    {
      if(Key.Type.IsView)
      {
        try
        {
          _viewExchange.PushUpdate((View) Instance);
        }
        catch(Exception error)
        {
          Log.Error(error, "[timeline] View {Flow:l} failed to push update");
        }
      }
    }

    private void PushStopped(Exception error)
		{
			Log.Error(error, "[timeline] Flow {Flow:l} stopped", Key);

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