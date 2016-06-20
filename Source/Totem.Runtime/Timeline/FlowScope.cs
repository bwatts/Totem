using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The scope of a flow's activity on the timeline
	/// </summary>
	public sealed class FlowScope : Connection, IFlowScope
	{
		private readonly TaskCompletionSource<Flow> _taskCompletionSource = new TaskCompletionSource<Flow>();
		private readonly ILifetimeScope _lifetime;
		private readonly TimelineScope _timeline;
		private readonly IViewExchange _viewExchange;
		private Flow _flow;
		private FlowQueue _queue;
		private Task _pushQueueTask;

		public FlowScope(ILifetimeScope lifetime, TimelineScope timeline, IViewExchange viewExchange, Flow flow)
		{
			_lifetime = lifetime;
			_timeline = timeline;
			_viewExchange = viewExchange;
			_flow = flow;
		}

		public FlowKey Key => _flow.Key;
		public Task<Flow> Task => _taskCompletionSource.Task;

		protected override void Open()
		{
			_queue = new FlowQueue();
			_pushQueueTask = PushQueue();
		}

		protected override void Close()
		{
			_queue = null;
			_pushQueueTask = null;
		}

		public void Push(TimelinePoint point)
		{
			if(PushingQueue)
			{
				_queue.Enqueue(point);
			}
			else
			{
				Log.Warning("[timeline] Flow {Flow:l} is done - ignoring {Point:l}", _flow, point);
			}
		}

		private bool PushingQueue => State.IsConnecting || State.IsConnected;

		private async Task PushQueue()
		{
			while(PushingQueue)
			{
				var point = await _queue.Dequeue();

				if(PushingQueue)
				{
					await PushNext(point);
				}
			}
		}

		private async Task PushNext(TimelinePoint point)
		{
			try
			{
				await MakeCall(point);
			}
			catch(Exception error)
			{
				if(Key.Type.IsRequest)
				{
					throw;
				}

				PushFlowStopped(point, error);
			}
		}

		private async Task MakeCall(TimelinePoint point)
		{
			Log.Verbose("[timeline] {Position:l} => {Flow:l}", point.Position, Key);

			using(var callScope = _lifetime.BeginCallScope())
			{
				var call = CreateCall(point, callScope);

				await _flow.MakeCall(call);

				if(!Key.Type.IsRequest)
				{
					_timeline.PushCall(call);
				}

				if(Key.Type.IsView)
				{
					PushViewUpdate();
				}

				if(_flow.Done)
				{
					CompleteTask();
				}
			}
		}

		private WhenCall CreateCall(TimelinePoint point, ILifetimeScope scope)
		{
			var dependencies = scope.Resolve<IDependencySource>();
			var principal = _timeline.ReadPrincipal(point);

			if(_flow.Type.IsTopic)
			{
				return new TopicWhenCall(
					(Topic) _flow,
					point,
					dependencies,
					principal,
					State.CancellationToken);
			}

			return new WhenCall(
				_flow,
				point,
				dependencies,
				principal,
				State.CancellationToken);
		}

		private void PushViewUpdate()
		{
			_viewExchange.PushUpdate((View) _flow);
		}

		private void PushFlowStopped(TimelinePoint point, Exception error)
		{
			Log.Error(error, "[timeline] Flow {Flow:l} stopped", _flow);

			try
			{
				_timeline.PushFlowStopped(Key, point, error);
			}
			finally
			{
				CompleteTask();
			}
		}

		private void CompleteTask()
		{
			_taskCompletionSource.TrySetResult(_flow);

			Disconnect();
		}
	}
}