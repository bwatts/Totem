using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The hosting of a flow on the timeline
	/// </summary>
	public sealed class FlowHost : TimelineScope, IFlowHost
	{
		private readonly TaskCompletionSource<Flow> _taskCompletionSource = new TaskCompletionSource<Flow>();
		private readonly ILifetimeScope _scope;
		private readonly IFlowDb _db;
		private Flow _instance;

		public FlowHost(FlowType type, ILifetimeScope scope, IFlowDb db)
		{
			Type = type;
			_scope = scope;
			_db = db;
		}

		public FlowType Type { get; private set; }
		public Task<Flow> Task { get { return _taskCompletionSource.Task; } }

		protected override void Open()
		{
			base.Open();

			_instance = _db.ReadInstance(Type);
		}

		protected override void Close()
		{
			base.Close();

			_instance = null;
		}

		protected override async Task CallWhen()
		{
			try
			{
				if(CanCall())
				{
					await MakeCallInScope();
				}
			}
			catch(Exception error)
			{
				if(Type.IsRequest)
				{
					throw;
				}

				AppendError(error);
			}
		}

		private bool CanCall()
		{
			return Point.Position > _instance.Checkpoint && Type.Events.Contains(Point.EventType);
		}

		private async Task MakeCallInScope()
		{
			using(var callScope = _scope.BeginCallScope())
			{
				await MakeCall(callScope);
			}
		}

		private async Task MakeCall(ILifetimeScope scope)
		{
			var call = CreateCall(scope);

			await _instance.CallWhen(call);

			_db.AppendCall(call);

			if(CheckDone())
			{
				Log.Verbose("[timeline] {Position:l} /> {Flow:l}", Point.Position, Type);
			}
			else
			{
				Log.Verbose("[timeline] {Position:l} => {Flow:l}", Point.Position, Type);
			}
		}

		private FlowCall CreateCall(ILifetimeScope scope)
		{
			return FlowCall.From(
				Type,
				_instance,
				Point,
				scope.Resolve<IDependencySource>(),
				_db.ReadPrincipal(Point),
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

		private void AppendError(Exception error)
		{
			Log.Error(error, "[timeline] Flow {Flow:l} stopped", _instance);

			try
			{
				_db.AppendError(Type, Point, error);
			}
			finally
			{
				CompleteTask();
			}
		}

		private void CompleteTask()
		{
			_taskCompletionSource.TrySetResult(_instance);

			_instance = null;

			Disconnect();
		}
	}
}