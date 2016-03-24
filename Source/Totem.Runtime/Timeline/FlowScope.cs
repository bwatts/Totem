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
	public sealed class FlowScope : PushScope, IFlowScope
	{
		private readonly TaskCompletionSource<Flow> _taskCompletionSource = new TaskCompletionSource<Flow>();
		private readonly ILifetimeScope _lifetime;
		private readonly IFlowDb _db;
		private readonly FlowType _type;
		private TimelineRoute _route;
    private Flow _flow;

		public FlowScope(ILifetimeScope lifetime, IFlowDb db, FlowType type, TimelineRoute route)
		{
			_lifetime = lifetime;
			_db = db;
			_type = type;
			_route = route;
			Key = type.CreateKey(route.Id);
		}

    public FlowKey Key { get; }
    public Task<Flow> Task => _taskCompletionSource.Task;

		internal bool TryRoute()
		{
			var route = _route;

			_route = null;

			return _db.TryReadFlow(_type, route, out _flow);
		}

		protected override void Push()
		{
			try
			{
				if(PointIsPastCheckpoint)
				{
          MakeCall();
				}
			}
			catch(Exception error)
			{
				if(_type.IsRequest)
				{
					throw;
				}

				WriteError(error);
			}
		}

    private bool PointIsPastCheckpoint => Point.Position > _flow.Checkpoint;

    private void MakeCall()
    {
      MakeCallAsync().Wait(State.CancellationToken);
    }

		private async Task MakeCallAsync()
		{
			using(var callScope = _lifetime.BeginCallScope())
			{
        var call = CreateCall(callScope);

        await _flow.MakeCall(call);

        _db.WriteCall(call);

        if(CheckDone())
        {
          Log.Verbose("[timeline] {Position:l} /> {Flow:l}", Point.Position, Key);
        }
        else
        {
          Log.Verbose("[timeline] {Position:l} => {Flow:l}", Point.Position, Key);
        }
      }
		}

		private WhenCall CreateCall(ILifetimeScope scope)
		{
      var dependencies = scope.Resolve<IDependencySource>();
      var principal = _db.ReadPrincipal(Point);

      if(_type.IsTopic)
      {
        return new TopicWhenCall(
          (Topic) _flow,
          Point,
          dependencies,
          principal,
          State.CancellationToken);
      }

      return new WhenCall(
				_flow,
				Point,
        dependencies,
        principal,
				State.CancellationToken);
		}

		private bool CheckDone()
		{
			if(_flow.Done)
			{
				CompleteTask();

				return true;
			}

			return false;
		}

		private void WriteError(Exception error)
		{
			Log.Error(error, "[timeline] Flow {Flow:l} stopped", _flow);

			try
			{
				_db.WriteError(Key, Point, error);
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