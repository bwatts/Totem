using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A queue of timeline points with asynchronous dequeues for the whole timeline
	/// </summary>
	internal sealed class TimelineQueue : Connection
	{
		private readonly Subject<TimelinePoint> _points = new Subject<TimelinePoint>();
		private readonly SortedDictionary<TimelinePosition, TimelinePoint> _futurePoints = new SortedDictionary<TimelinePosition, TimelinePoint>();
		private readonly TimelineSchedule _schedule;
		private readonly TimelineFlowSet _flows;
		private readonly TimelineRequestSet _requests;
		private TimelinePosition _nextPosition;

		internal TimelineQueue(TimelineSchedule schedule, TimelineFlowSet flows, TimelineRequestSet requests)
		{
			_schedule = schedule;
			_flows = flows;
			_requests = requests;
		}

		protected override void Open()
		{
			Track(_points
				.ObserveOn(ThreadPoolScheduler.Instance)
				.Subscribe(point =>
				{
					_schedule.Push(point);
					if (!point.Scheduled)
					{
						_flows.Push(point);
						_requests.Push(point);
					}
				}));
		}

		internal void ResumeWith(ResumeInfo info)
		{
			_nextPosition = info.NextPosition;

			foreach(var pointInfo in info.Points)
			{
				_points.OnNext(pointInfo.Point);
			}
		}

		internal void Enqueue(TimelinePoint point)
		{
			State.ExpectConnected();

			lock(_points)
			{
				ExpectNotPushed(point);

				if(CheckNext(point))
				{
					EnqueueNext(point);

					EnqueueFuturePoints();
				}
			}
		}

		private void ExpectNotPushed(TimelinePoint point)
		{
			if(point.Position < _nextPosition)
			{
				throw new InvalidOperationException($"Cannot enqueue previously-pushed position {point.Position}; expected {_nextPosition}");
			}
		}

		private bool CheckNext(TimelinePoint point)
		{
			if(point.Position == _nextPosition)
			{
				return true;
			}

			_futurePoints.Add(point.Position, point);

			return false;
		}

		private void EnqueueNext(TimelinePoint point)
		{
			_nextPosition = point.Position.Next();

			_points.OnNext(point);
		}

		private void EnqueueFuturePoints()
		{
			foreach(var futurePoint in _futurePoints.ToList())
			{
				var position = futurePoint.Key;
				var point = futurePoint.Value;

				if(position != _nextPosition)
				{
					break;
				}

				_futurePoints.Remove(position);

				_nextPosition = position.Next();

				_points.OnNext(point);
			}
		}
	}
}