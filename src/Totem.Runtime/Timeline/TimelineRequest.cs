using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A request pending a response on the timeline
	/// </summary>
	internal abstract class TimelineRequest : Notion
	{
		internal TimelineRequest(IFlowHost flowHost)
		{
			FlowHost = flowHost;

			FlowHost.Task.ContinueWith(_ => Respond());
		}

		protected readonly IFlowHost FlowHost;

		internal void Push(TimelinePoint point)
		{
			FlowHost.Push(point);
		}

		private void Respond()
		{
			if(FlowHost.Task.IsCompleted)
			{
				RespondCompleted(FlowHost.Task.Result);
			}
			else if(FlowHost.Task.IsFaulted)
			{
				RespondError(FlowHost.Task.Exception);
			}
			else
			{
				RespondCancelled();
			}
		}

		internal abstract void RespondCompleted(Flow instance);

		internal abstract void RespondError(Exception error);

		internal abstract void RespondCancelled();
	}

	/// <summary>
	/// A request pending a response on the timeline
	/// </summary>
	/// <typeparam name="TFlow">The type of flow observing the request</typeparam>
	internal sealed class TimelineRequest<TFlow> : TimelineRequest where TFlow : RequestFlow
	{
		private readonly TaskCompletionSource<TFlow> _taskCompletionSource = new TaskCompletionSource<TFlow>();

		internal TimelineRequest(IFlowHost flowHost) : base(flowHost)
		{}

		internal Task<TFlow> Task { get { return _taskCompletionSource.Task; } }

		internal override void RespondCompleted(Flow instance)
		{
			_taskCompletionSource.SetResult((TFlow) instance);
		}

		internal override void RespondError(Exception error)
		{
			_taskCompletionSource.SetException(error);
		}

		internal override void RespondCancelled()
		{
			_taskCompletionSource.SetCanceled();
		}
	}
}