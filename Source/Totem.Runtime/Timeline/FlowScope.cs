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
	public sealed class FlowScope : PushScope, IFlowScope
	{
		private readonly TaskCompletionSource<Flow> _taskCompletionSource = new TaskCompletionSource<Flow>();
		private readonly ILifetimeScope _scope;
		private readonly IFlowDb _db;
    private Flow _instance;

		public FlowScope(FlowKey key, ILifetimeScope scope, IFlowDb db)
		{
      Key = key;
			_scope = scope;
			_db = db;
		}

    public FlowKey Key { get; }
    public Task<Flow> Task => _taskCompletionSource.Task;

		protected override void Open()
		{
      _instance = _db.ReadInstance(Key);

      base.Open();
		}

		protected override void Close()
		{
			base.Close();

			_instance = null;
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
				if(Key.Type.IsRequest)
				{
					throw;
				}

				WriteError(error);
			}
		}

    private bool PointIsPastCheckpoint => Point.Position > _instance.Checkpoint;

    private void MakeCall()
    {
      MakeCallAsync().Wait(State.CancellationToken);
    }

		private async Task MakeCallAsync()
		{
			using(var callScope = _scope.BeginCallScope())
			{
        var call = CreateCall(callScope);

        await _instance.MakeCall(call);

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

      if(_instance.Type.IsTopic)
      {
        return new TopicWhenCall(
          (Topic) _instance,
          Point,
          dependencies,
          principal,
          State.CancellationToken);
      }

      return new WhenCall(
				_instance,
				Point,
        dependencies,
        principal,
				State.CancellationToken);
		}

		private bool CheckDone()
		{
			if(_instance.Done)
			{
				CompleteTask();

				return true;
			}

			return false;
		}

		private void WriteError(Exception error)
		{
			Log.Error(error, "[timeline] Flow {Flow:l} stopped", _instance);

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
			_taskCompletionSource.TrySetResult(_instance);

			Disconnect();
		}
	}
}