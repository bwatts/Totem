using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A request pending a response on the timeline
	/// </summary>
	internal abstract class TimelineRequest : Connection
	{
		internal TimelineRequest(IFlowScope flow)
		{
			Flow = flow;
		}

    internal readonly IFlowScope Flow;

    internal abstract bool Responded { get; }

    protected override void Open()
    {
      Flow.Task.ContinueWith(_ => Respond());

      Track(Flow);

      base.Open();
    }

		internal void Push(TimelineMessage message)
		{
			if(Flow.Key.Type.Events.Contains(message.Point.EventType))
			{
        var route = new FlowRoute(Flow.Key, first: false, when: true, given: false, then: false);

        Flow.Push(new FlowPoint(route, message.Point));
      }
		}

    internal void PushError(Exception error)
    {
      if(Responded)
      {
        return;
      }

      try
      {
        RespondError(error);
      }
      finally
      {
        Close();
      }
    }

    private void Respond()
    {
      if(Responded)
      {
        return;
      }

      try
      {
        RespondFromTask();
      }
      finally
      {
        Close();
      }
    }

		private void RespondFromTask()
		{
			if(Flow.Task.IsCompleted)
			{
				RespondCompleted(Flow.Instance);
			}
			else if(Flow.Task.IsFaulted)
			{
				RespondError(Flow.Task.Exception);
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
	/// <typeparam name="T">The type of observing request</typeparam>
	internal sealed class TimelineRequest<T> : TimelineRequest where T : Request
	{
		private readonly TaskCompletionSource<T> _taskCompletionSource = new TaskCompletionSource<T>();

		internal TimelineRequest(IFlowScope flow) : base(flow)
		{}

		internal Task<T> Task => _taskCompletionSource.Task;

    internal override bool Responded => Task.IsCompleted || Task.IsFaulted || Task.IsCanceled;

		internal override void RespondCompleted(Flow instance)
		{
			_taskCompletionSource.SetResult((T) instance);
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