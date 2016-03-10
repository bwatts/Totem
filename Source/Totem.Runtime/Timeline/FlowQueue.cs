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
		private TaskCompletionSource<TimelinePoint> _pendingRead;

		internal void Enqueue(TimelinePoint point)
		{
			TaskCompletionSource<TimelinePoint> pendingRead = null;
			
			lock(_pendingPoints)
			{
				if(_pendingRead != null)
				{
					pendingRead = _pendingRead;

					_pendingRead = null;
				}
				else
				{
					_pendingPoints.Enqueue(point);
				}
			}

			if(pendingRead != null)
			{
				pendingRead.TrySetResult(point);
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

				_pendingRead = new TaskCompletionSource<TimelinePoint>();

				return _pendingRead.Task;
			}
		}
	}
}