using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A queue of timeline points with asynchronous dequeues for a single flow
	/// </summary>
	internal sealed class FlowQueue
	{
		private readonly Queue<TimelinePoint> _pendingPoints = new Queue<TimelinePoint>();
		private TaskCompletionSource<TimelinePoint> _pendingDequeue;

		internal void Enqueue(TimelinePoint point)
		{
			TaskCompletionSource<TimelinePoint> pendingDequeue = null;
			
			lock(_pendingPoints)
			{
				if(_pendingDequeue != null)
				{
					pendingDequeue = _pendingDequeue;

					_pendingDequeue = null;
				}
				else
				{
					_pendingPoints.Enqueue(point);
				}
			}

			if(pendingDequeue != null)
			{
				pendingDequeue.TrySetResult(point);
			}
		}

		internal Task<TimelinePoint> Dequeue()
		{
			lock(_pendingPoints)
			{
				if(_pendingPoints.Count > 0)
				{
					return Task.FromResult(_pendingPoints.Dequeue());
				}

				_pendingDequeue = new TaskCompletionSource<TimelinePoint>();

				return _pendingDequeue.Task;
			}
		}
	}
}